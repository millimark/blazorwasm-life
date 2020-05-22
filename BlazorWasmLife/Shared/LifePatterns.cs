using System;
using System.Collections.Generic;

namespace BlazorWasmLife.Shared
{
    /// <summary>
    /// Defines well-known initial Life patterns 
    /// </summary>
    static public class LifePatterns
    {

        private static Dictionary<string, List<string>> patterns { get; } = InitPatterns();

        static private Dictionary<string, List<string>> InitPatterns()
        {
            Dictionary<string, List<string>> patternDict
                = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var initialPatterns = new KeyValuePair<string, List<string>>[15]
            {
                                new KeyValuePair<string, List<string>>( "Blank",
                    new List<string>(){
                            "00000",
                            "00000",
                            "00000",
                            "00000",
                            "00000"}
                    ),
                new KeyValuePair<string, List<string>>( "Block",
                    new List<string>(){
                            "0000",
                            "0110",
                            "0110",
                            "0000" }
                    ),

                    new KeyValuePair<string, List<string>>(
                "Beehive",
                new List<string>(){
                            "000000",
                        "001100",
                        "010010",
                        "001100",
                        "000000" }
                    ),
                    new KeyValuePair<string, List<string>>(
                "Loaf",
                new List<string>(){
                            "000000",
                        "001100",
                        "010010",
                        "001010",
                        "000100",
                        "000000" }
            ),
                    new KeyValuePair<string, List<string>>(
                "Boat",
                new List<string>(){
                            "00000",
                        "01100",
                        "01010",
                        "00100",
                        "00000" }

                    ),
                    new KeyValuePair<string, List<string>>(
                "Tub",
                new List<string>(){
                            "00000",
                        "00100",
                        "01010",
                        "00100",
                        "00000" }

               ),
                new KeyValuePair<string, List<string>>(
                "Blinker",
                new List<string>(){
                            "00000",
                        "00100",
                        "00100",
                        "00100",
                        "00000" }

                   ),
                new KeyValuePair<string, List<string>>(
                "Toad",
                new List<string>(){
                            "000000",
                        "000000",
                        "001110",
                        "011100",
                        "000000",
                        "000000" }
                    ),
                new KeyValuePair<string, List<string>>(
                "Beacon",new List<string>(){
                            "000000",
                        "011000",
                        "011000",
                        "000110",
                        "000110",
                        "000000" }
                    ),
                  new KeyValuePair<string, List<string>>(
                "Pulsar",new List<string>(){
                           "00000000000000000",
                       "00000100000100000",
                       "00000100000100000",
                       "00000110001100000",
                       "00000000000000000",
                       "01110011011001110",
                       "00010101010101000",
                       "00000110001100000",
                       "00000000000000000",
                       "00000110001100000",
                       "00010101010101000",
                       "01110011011001110",
                       "00000000000000000",
                       "00000110001100000",
                       "00000100000100000",
                       "00000100000100000",
                       "00000000000000000" }
                    ),
                 new KeyValuePair<string, List<string>>(
                "Pentadecathlon",new List<string>(){
                                     "00000000000",
                                     "00000000000",
                                     "00000000000",
                                     "00001110000",
                                     "00000100000",
                                     "00000100000",
                                     "00001110000",
                                     "00000000000",
                                     "00001110000",
                                     "00001110000",
                                     "00000000000",
                                     "00001110000",
                                     "00000100000",
                                     "00000100000",
                                     "00001110000",
                                     "00000000000",
                                     "00000000000",
                                     "00000000000" }
                    )
                    ,
                 new KeyValuePair<string, List<string>>(
                "Glider",new List<string>(){
                        "0000000",
                    "0001000",
                    "0000100",
                    "0011100",
                    "0000000",
                    "0000000" }),
                new KeyValuePair<string, List<string>>(
                "LWSS",new List<string>(){
                        "000000000",
                    "001001000",
                    "000000100",
                    "001000100",
                    "000111100",
                    "000000000",
                    "000000000" } ),
                new KeyValuePair<string, List<string>>(
                "MWSS",new List<string>(){
                        "0000000000",
                    "0000000000",
                    "0000000000",
                    "0001111100",
                     "0010000100",
                     "0000000100",
                     "0010001000",
                     "0000100000",
                     "0000000000" } ),
                new KeyValuePair<string, List<string>>(
                "HWSS",new List<string>(){
                        "00000000000",
                    "00000000000",
                    "00000000000",
                    "00011111100",
                     "00100000100",
                     "00000000100",
                     "00100001000",
                     "00001100000",
                     "00000000000"
                })
                };
            foreach (var p in initialPatterns)
            {
                patternDict.Add(p.Key, p.Value);
            }

            return patternDict;
        }


        static public List<string> GetPattern(string name)
        {
            return patterns[name];
        }

    }
}
