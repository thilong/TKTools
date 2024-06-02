using System.Reflection;

namespace TKCliTool.Tools;

[ToolAttribute("webdav", "tools for uploading files to webdav server.")]
public class WebdavUploadTool : ToolBase
{
    public override void printHelp()
    {
        Console.WriteLine($"Usage: dotnet {AssemblyName}.dll webdav [options] [arguments]");
        Console.WriteLine("Options:");
        var methods = this.GetType().GetMethods();
        var actions = methods
            .Where(t => t.GetCustomAttribute<ActionAttribute>() != null)
            .ToList();
        actions.ForEach(t =>
        {
            var attr = t.GetCustomAttribute<ActionAttribute>();
            if (attr != null)
                Console.WriteLine($"\t{attr.ShortAction}\t{attr.Action}\t{attr.Arguments}\n\t    {attr.Description}");
        });
    }

    public override void Run(string[] args)
    {
            
    }
}
