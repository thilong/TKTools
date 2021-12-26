using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TKWebdavUploader;

class UploadExclude{

    public virtual bool CanUpload(string remotePath){
        return true;
    }

    public static UploadExclude Parse(string partten){
        if(partten.StartsWith("endwith", true, null))
            return new UploadExcludeEndWith(partten);
        if(partten.StartsWith("contains", true,null))
            return new UploadExcludeContains(partten);
        return new UploadExclude();
    }

}

class UploadExcludeEndWith : UploadExclude{
    private string partten = "";
    public UploadExcludeEndWith(string parttenStr){
        partten=parttenStr.Replace("EndWith:","", true, null).Trim();
    }

    public override bool CanUpload(string remotePath){
        return !remotePath.EndsWith(partten, true, null);
    }
}

class UploadExcludeContains : UploadExclude{
    private string partten = "";

    public override bool CanUpload(string remotePath){
        return !remotePath.Contains(partten,StringComparison.OrdinalIgnoreCase);
    }

    public UploadExcludeContains(string parttenStr){
        partten=parttenStr.Replace("contains:","", true, null).Trim();
    }

}


public class UploadOption
{

    //是否可以处理Webdav,需要在工作目录下有“.tk.webdav”这个配置文件
    [JsonIgnore]
    public bool CanProcess => (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Password));


    public string Server { get; set; }


    public string RootPath { get; set; } = "/";

    [JsonPropertyName("Uid")]
    public string UserId { get; set; }

    [JsonPropertyName("Pwd")]
    public string Password { get; set; }

    [JsonPropertyName("Reboot")]
    public string RebootPath { get; set; }

    [JsonPropertyName("Exclude")]
    public List<string> Excludes { get; set; }

    private List<UploadExclude> excludeCheckers;
    
    public static UploadOption Parse(string workingFolder, string shadowOp = null)
    {
        var configName = string.IsNullOrWhiteSpace(shadowOp) ? ".tk.webdav" : $".tk.webdav.{shadowOp}";
        var optionFile = Path.Join(workingFolder, configName);
        if (!File.Exists(optionFile))
            return new UploadOption();

        var content = File.ReadAllText(optionFile);
        var ret = System.Text.Json.JsonSerializer.Deserialize<UploadOption>(content);
        if (ret != null)
        {
            if (!ret.RootPath.EndsWith("/"))
                ret.RootPath = ret.RootPath + "/";
            if(ret.Excludes != null){
                ret.excludeCheckers = ret.Excludes.Select(m => UploadExclude.Parse(m)).ToList();
            }
            return ret;
        }
        return new UploadOption();
    }

    public bool CanUpload(string remotePath){
        if(excludeCheckers == null || excludeCheckers.Count == 0) 
            return true;
            
        foreach(var checker in excludeCheckers){
            if(!checker.CanUpload(remotePath)){
                return false;
            }
        }

        return true;
    }

}