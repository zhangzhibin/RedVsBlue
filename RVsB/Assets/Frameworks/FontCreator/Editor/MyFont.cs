//using UnityEngine;
//using System.Collections;
//using UnityEditor; 
//
//public class MyFont : MonoBehaviour {
//    public Font m_myFont;
//    public TextAsset m_data;
//    private BMFont mbFont = new BMFont();
//    void Start()
//    {
//        BMFontReader.Load(mbFont, m_data.name, m_data.bytes);
//        CharacterInfo[] characterInfo = new CharacterInfo[mbFont.glyphs.Count];
//        for (int i = 0; i < mbFont.glyphs.Count; i++)
//        {
//            BMGlyph bmInfo = mbFont.glyphs[i];
//            CharacterInfo info = new CharacterInfo();
//            info.index = bmInfo.index;
////             float x = (float)bmInfo.x / (float)mbFont.texWidth;
////             float y = 1 - (float)bmInfo.y / (float)mbFont.texHeight;
//           // info.uvBottomLeft = new Vector2(x, y);
//            info.uv.x = (float)bmInfo.x / (float)mbFont.texWidth;
//            info.uv.y = 1 - (float)bmInfo.y / (float)mbFont.texHeight;
//            info.uv.width = (float)bmInfo.width / (float)mbFont.texWidth;
//            info.uv.height = -1f * ((float)bmInfo.height )/ (float)mbFont.texHeight;
//            info.vert.x = (float)bmInfo.offsetX; 
//            info.vert.y = 0f;
//            info.vert.width = (float)bmInfo.width;
//            info.vert.height = (float)bmInfo.height;
//            info.width = (float)bmInfo.advance;
//            characterInfo[i] = info;
//        }
//        m_myFont.characterInfo = characterInfo;
//    }  
//}
