using System.Diagnostics.CodeAnalysis;

namespace TKWebdavUploader
{

    class TKWebDav
    {

        private string workingFolder;

        private UploadOption option;

        private UploadRecord record;


        private UploadWorker worker;
        private bool anyFileUploaded = false;

        public TKWebDav(string workingFolder)
        {
            if (workingFolder.EndsWith("/"))
                this.workingFolder = workingFolder;
            else
                this.workingFolder = workingFolder + "/";
        }

        public async Task Run(string shadowOp = null)
        {
            Console.WriteLine($"-- Working in {workingFolder}");
            if (string.IsNullOrWhiteSpace(workingFolder) || !Directory.Exists(workingFolder))
            {
                Console.WriteLine($"--Working path '{workingFolder}' not exists!");
                return;
            }
            option = UploadOption.Parse(workingFolder, shadowOp);
            if (!option.CanProcess)
            {
                var configName = string.IsNullOrWhiteSpace(shadowOp) ? ".tk.webdav" : $".tk.webdav.{shadowOp}";
                Console.WriteLine($"-- No config file found, config file name is '{configName}', exit!!!");
                return;
            }

            record = new UploadRecord(shadowOp);
            record.Load(workingFolder);

            worker = new UploadWorker();
            worker.Connect(Path.Join(option.Server), option.UserId, option.Password);
            var totalError = await RunInFolder(workingFolder);
            record.Save(workingFolder);
            Console.WriteLine($"-- Done, {totalError} file upload failed.");

            if (!anyFileUploaded)
            {
                Console.WriteLine("xx No file uploaded, no need to reboot.");
                return;
            }

            if (totalError == 0 && !string.IsNullOrWhiteSpace(option.RebootPath))
            {
                Console.WriteLine("-- Rebooting ...");
                //如果文件全部上传完，则重启
                using var hc = new HttpClient();
                var response = await hc.GetAsync(option.RebootPath);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Reboot]: {response.StatusCode},{data}");
                }
                else
                {
                    Console.WriteLine($"[Reboot]: failed, {response.StatusCode}");
                }
            }
            else
            {
                Console.WriteLine("xx Can't reboot the system.");
            }
        }

        private async Task<int> RunInFolder(string folder)
        {
            Console.WriteLine($"** In Folder : {folder}");
            var error = 0;

            //process all files
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                if (!await ProcessFile(file))
                    error += 1;
            }

            //process sub directories
            var subFolders = Directory.GetDirectories(folder);
            foreach (var subFolderEmu in subFolders)
            {
                var subFolder = subFolderEmu;
                if (!subFolder.EndsWith("/"))
                    subFolder = subFolder + "/";

                if (!option.CanUpload(subFolder))
                {
                    Console.WriteLine($"[Exclude Folder]: {subFolder}");
                    continue;
                }
                var subError = await RunInFolder(subFolder);
                error += subError;
            }

            return error;
        }

        private async Task<bool> ProcessFile(string file)
        {
            //不处理webdav的配置文件
            if (file.Contains("tk.webdav"))
                return true;

            var remotePath = option.RootPath + file.Replace(workingFolder, "");
            var nowTimestamp = (long)DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds;
            //检测配置
            if (!option.CanUpload(remotePath))
            {
                //Console.WriteLine($"[Exclude]: {remotePath}");
                return true;
            }
            //检测上传时间
            if (!record.ShouldUpload(file, remotePath))
            {
                //Console.WriteLine($"[Skipped]: {remotePath}");
                return true;
            }

            var ret = await worker.Upload(file, remotePath);

            if (ret)
            {
                record.Record(remotePath, nowTimestamp);
                Console.WriteLine($"[Uploaded] {remotePath} ...");
                anyFileUploaded = true;
            }
            else
            {
                Console.WriteLine($"[Failed] {remotePath} ~~~");
            }

            return ret;
        }
    }
}