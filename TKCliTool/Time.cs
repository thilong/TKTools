namespace TKCliTool;

public class Time
{

    public delegate void messageHandler(string[] args);

    public static void printHelp(){
        Console.WriteLine("Usage: TKCliTool time <command> [options]");
        Console.WriteLine("Commands:");
        Console.WriteLine("  now\t\t\tprint the current time");
        Console.WriteLine("  timestamp\t\tprint the current timestamp");
    }

    Dictionary<string, Action<string[]>> functions = new Dictionary<string, Action<string[]>>();
    private void buildFunctionMap(){
        functions.Add("now", new Action<string[]>(onNow));
        functions.Add("timestamp", new Action<string[]>(onTimestamp));
    }

    public void Run(string[] args){
        buildFunctionMap();

        var subcommand = args.Length > 1 ? args[1] : "";
        if(functions.ContainsKey(subcommand)){
            functions[subcommand].Invoke(args[2..]);
        }else{
            printHelp();
        }
    }

    public void onNow(string[] args){
        Console.WriteLine(DateTime.Now + string.Join(",", args));
    }


    public void onTimestamp(string[] args){
        Console.WriteLine((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }
}
