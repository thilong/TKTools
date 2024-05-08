namespace TKCliTool;

public class Help
{
    
    public static void printHelp()
    {
        Console.WriteLine("Usage: TKCliTool <command> [options]");
        Console.WriteLine("Commands:");
        Console.WriteLine("  time");
        Console.WriteLine("       use the time command");
        Console.WriteLine("  help [subcommand]");
        Console.WriteLine("       print help for a subcommand, empty for this help");
    }


    public void Run(string[] args){
        if(args.Length == 1){
            printHelp();
            return;
        }
        var subcommand = args[1];
        switch(subcommand){
            case "time":
                Time.printHelp();
                return;
            default:
                printHelp();
                return;
        }
    }


}
