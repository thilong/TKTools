using System.Reflection;

namespace TKCliTool.Tools;

[ToolAttribute("help", "print help for all commands")]
public class UsageTool : ToolBase
{
    public override void printHelp()
    {
        //get all class with ToolAttribute
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<ToolAttribute>() != null)
            .ToList();
            


        Console.WriteLine($"Usage: dotnet {AssemblyName}.dll <command> [options]");
        Console.WriteLine("Commands:");
        types.ForEach(t =>
        {
            var attr = t.GetCustomAttribute<ToolAttribute>();
            if(attr != null)
                Console.WriteLine($"\t{attr.Name,-10}\n\t    {attr.Description}");
        });
        Console.WriteLine("\thelp <command>");
        Console.WriteLine("\t    print help for a subcommand, empty for this help");
    }

    public override void Run(string[] args)
    {
        this.printHelp();
    }
}

