// using System;
// using System.Linq;
// using BFS.Abstractions;
// using BFS.LastLevelOpt;
// using BFS.NoOpt;
// using BFS.SickPropagation;
// using BFS.SickPropagationGraphOpt;

// namespace BFS
// {
//     static class Program
//     {
//         public static void Main(string[] args)
//         {
//             if (!args.Any())
//             {
//                 Console.WriteLine("Please select an algorithm");
//             }

//             var alg = args.First();

//             IBFS bfs = alg switch
//             {
//                 "noopt" => new BfsNoOpt(),
//                 "last" => new BfsLastLevelOpt(),
//                 "sick" => new BfsSickPropagation(),
//                 "sickOpt" => new BfsSickPropagationGraphOpt(),
//                 _ => throw new NotImplementedException()
//             };

//             bfs.Execute();
//         }
//     }
// }