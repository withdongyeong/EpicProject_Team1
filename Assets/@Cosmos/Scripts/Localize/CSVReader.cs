using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CSVReader : Singleton<CSVReader>
{
    public List<string[]> NotObjectText_List = new List<string[]>();

    protected override void Awake()
    {
        base.Awake();  // 싱글톤 체크

        string NotObjectText_CSV = Path.Combine(Application.streamingAssetsPath, "오브젝트외텍스트.csv");

        FileToString(NotObjectText_CSV, NotObjectText_List);
    }

    private void FileToString(string filePath, List<string[]> StoryText)
    {
        if (File.Exists(filePath))
        {
            // UTF-8 인코딩으로 파일 읽기
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                StoryText.Add(values);
            }
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다!");
        }
    }
}
