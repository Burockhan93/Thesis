using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TsAPI.Types;
using UnityEngine;

public class Extractor
{
    private static readonly string _path = Application.dataPath + @"/ExtractedFiles";
    public static void WriteToFile(string _filename, StringBuilder stringBuilder)
    {
       
        
        File.WriteAllText((_path + @$"/{_filename}.txt"), stringBuilder.ToString());

    }
}
   
   

