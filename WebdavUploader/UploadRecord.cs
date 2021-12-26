namespace TKWebdavUploader;

class UploadRecord{
    
    private Dictionary<string, long> fileTimespans = new();

    public bool ShouldUpload(string localFile, string file){
        //获取文件的最后更新时间
        var lastUpdateTime = (long)File.GetLastWriteTime(localFile).Subtract(DateTime.UnixEpoch).TotalSeconds;
        if(fileTimespans.ContainsKey(file))
            return fileTimespans[file] < lastUpdateTime;
        return true;
    }

    private string configName;
    public UploadRecord(string shadowOp){
        if(string.IsNullOrWhiteSpace(shadowOp))
            configName = ".tk.webdavrecord";
        else
            configName = $".tk.webdavrecord.{shadowOp}";
    }

    public void Load(string workFolder){
        var recordFile = Path.Join(workFolder, configName);
        if(!File.Exists(recordFile))
            return;
        var fileContent = File.ReadAllText(recordFile);
        var records = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, long>>(fileContent);
        if(records != null ){
            fileTimespans = records;
        }
    }

    public void Save(string workFolder){
        if(fileTimespans.Count <= 0) return;
        var content = System.Text.Json.JsonSerializer.Serialize(fileTimespans);
        var recordFile = Path.Join(workFolder, configName);
        File.WriteAllText(recordFile, content);
    }

    public void Record(string file, long timestamp){
        fileTimespans[file] = timestamp;
    }

}