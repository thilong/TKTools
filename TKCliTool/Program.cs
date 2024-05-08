global using TKCliTool;


var argCount = args.Length;

//run different code based on the number of arguments
if(argCount == 0){
    Help.printHelp();
    return;
}

var subcommand = args[0];
switch(subcommand){
    case "time":
        new Time().Run(args);
        return;
    case "help":
        new Help().Run(args);
        return;
    default:
        Help.printHelp();
        return;
}
