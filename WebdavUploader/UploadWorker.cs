namespace TKWebdavUploader;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using WebDav;

class UploadWorker
{

    
    private IWebDavClient webdav;

    ~UploadWorker()
    {
        if (webdav != null)
            webdav.Dispose();
    }

    public void Connect(string path, string uid, string pwd)
    {
        var webDavParam = new WebDavClientParams
        {
            BaseAddress = new Uri(path),
            Credentials = new NetworkCredential(uid, pwd)
        };
        webdav = new WebDavClient(webDavParam);
    }

    public async Task<bool> Upload(string file, string toPath)
    {
        if(webdav == null) return false;
        var response = await webdav.PutFile(toPath, File.OpenRead(file));
        return response.IsSuccessful;
    }


}