// See https://aka.ms/new-console-template for more information
global using System;
global using System.Linq;

if(args.Count() < 1){
    Console.WriteLine("No working path set, can't run.");
    return;
}

if(args.Count() == 1)
    await (new TKWebdavUploader.TKWebDav(args[0]).Run());
if(args.Count() == 2)
    await (new TKWebdavUploader.TKWebDav(args[0]).Run(args[1]));
Console.WriteLine("End");
