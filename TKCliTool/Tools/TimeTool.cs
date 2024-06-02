using System.Reflection;

namespace TKCliTool.Tools;

[ToolAttribute("time", "tools for time, date and timestamp")]
public class TimeTool : ToolBase
{
    public override void printHelp()
    {
        Console.WriteLine($"Usage: dotnet {AssemblyName}.dll time [options] [arguments]");
        Console.WriteLine("Options:");
        var methods = this.GetType().GetMethods();
        var actions = methods
            .Where(t => t.GetCustomAttribute<ActionAttribute>() != null)
            .ToList();
        actions.ForEach(t =>
        {
            var attr = t.GetCustomAttribute<ActionAttribute>();
            if (attr != null)
                Console.WriteLine($"\t{attr.ShortAction}\t{attr.Action}\t{attr.Arguments}\n\t    {attr.Description}\n");
        });
    }

    public override void Run(string[] args)
    {
        var methods = this.GetType().GetMethods();
        var actions = methods
            .Where(t => t.IsEqualToAction(args[0]))
            .FirstOrDefault();
        if (actions == null)
        {
            Console.WriteLine("Invalid option. Use --help for more information.");
            return;
        }
        actions.Invoke(this, args.Skip(1).ToArray());
    }


    [ActionAttribute("--timestamp", "-ts", "print current timestamp")]
    public void PrintTimestamp(string[] args)
    {
        var timestamp = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        Console.WriteLine($"Current timestamp: {timestamp}");
    }

    [ActionAttribute("--date", "-d", "convert timestamp to date time string", "{timestamp}")]
    public void ConvertTimestampToDate(string[] args)
    {
        var timestamp = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        Console.WriteLine($"Current timestamp: {timestamp}");
    }

}
