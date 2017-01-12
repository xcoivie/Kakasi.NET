using System.Diagnostics;
using Kakasi.NET.Interop;
using System.Collections.Generic;
using System.Linq;
namespace Kakasi.NET.TestApp
{

    /// <summary>
    /// Test program
    /// </summary>
    class Program
    {

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {// Initialize library
            KakasiLib.Init();

            // Set params to get Furigana
            // NOTE: Use EUC-JP encoding as the wrapper will encode/decode using it
            KakasiLib.SetParams(new[] { "kakasi", "-ieuc", "-f", "-JH", "-w" });
            //KakasiLib.SetParams(new[] { "kakasi", "-ieuc", "-w" });

            // Get furigana from this book: アンブラとパン先生 by 伊藤 綾子
            // http://itunes.apple.com/us/book/id1144287620

            // Set source sentence
            //
            // Get furigana
            int counter = 0;
            string line;
            List<string> data = new List<string>();

            // Read the file and display it line by line.
            using (System.IO.StreamReader file =
               new System.IO.StreamReader(@"D:\temp\queryUpdateForSearchTag_9_shorter.sql"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    var resultValue2 = System.Text.RegularExpressions.Regex.Replace(line, @"[％：、。○★『』◯・（）「」／＆！＋？《》°…’＜＞〈〉【】～〜×❌!@#$%^&*()_\+={}\\|:;'<>?]", " ");
                    resultValue2 = resultValue2.ToLower().Replace("　", " ").Replace("  ", " ").Replace(" ", ",").Replace(",,", ",");
                    //System.Text.RegularExpressions.Regex.Replace(resultValue2, @"[％：、。○★『』◯・（）「」／＆！＋？《》°…’＜＞〈〉【】～〜×❌!@#$%^&*()_\+={}\\|:;'<>?]", " ");
                    var separatedList2 = resultValue2.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
                    var tastedId = separatedList2[0];
                    if (tastedId == "7pj0bdrqgl3o8")
                    {
                        ;
                    }
                    separatedList2[0] = string.Empty;
                    List<string> list2 = new List<string>();
                    list2.AddRange(separatedList2);

                    // Get furigana
                    var resultValue1 = KakasiLib.DoKakasi(line.ToLower());
                    //TODO
                    resultValue1 = System.Text.RegularExpressions.Regex.Replace(resultValue1, @"[％：、。○★『』◯・（）「」／＆！＋？《》°…’＜＞〈〉【】～〜×❌!@#$%^&*()_\+={}\\|:;'<>?]", " ");
                    resultValue1 = resultValue1.ToLower().Replace("　", " ").Replace("  ", " ").Replace(" ", ",").Replace(",,", ",");
                    var separatedList = resultValue1.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);

                    separatedList[0] = string.Empty;
                    List<string> distinctSeparatedList = new List<string>();
                    foreach (var kk in separatedList)
                    {
                        var str = kk.Replace(" ", string.Empty).Replace(",,", ",");
                        if (!string.IsNullOrEmpty(str)
                            && !distinctSeparatedList.Contains(str)
                            && !list2.Any(r => r.ToLower().IndexOf(str) == 0)
                            && !(
                            str.Length == 1 && !str.Any(r => (r >= 0x4E00 && r <= 0x9FBF)) /* kanji */
                            )
                            )
                        {
                            distinctSeparatedList.Add(str);
                        }
                    }
                    if (distinctSeparatedList.Count > 0)
                    {
                        string newSearchTag = string.Format("update Tasted set SearchTag = SearchTag + N',{0}' where TastedId = '{1}';", string.Join(",", distinctSeparatedList), tastedId);
                        data.Add(newSearchTag);
                    }
                    counter++;
                }
            }

            //Write file            
            using (System.IO.StreamWriter filew =
               new System.IO.StreamWriter(@"D:\temp\queryUpdateForSearchTag_9_shorter-converted.sql"))
            {
                foreach (string d in data)
                {
                    filew.WriteLine(d);
                }
            }

            KakasiLib.Dispose();

        }

    }
}
