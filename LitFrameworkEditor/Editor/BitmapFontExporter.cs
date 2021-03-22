using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System;

public class BitmapFontExporter : ScriptableWizard
{
    [MenuItem( "Tools/Build/BitmapFontExporter" )]
    private static void CreateFont()
    {
        ScriptableWizard.DisplayWizard<BitmapFontExporter>( "Create Font" );
    }

    /// <summary>
    /// BMFont字体文件
    /// </summary>
    [Header("BMFont字体文件")]
    public TextAsset fontFile;
    /// <summary>
    /// BMFont字体贴图
    /// </summary>
    [Header("BMFont字体贴图")]
    public Texture2D textureFile;

    private void OnWizardCreate()
    {
        if ( fontFile == null || textureFile == null )
        {
            return;
        }

        string path = EditorUtility.SaveFilePanelInProject( "Save Font", fontFile.name, "", "" );

        if ( !string.IsNullOrEmpty( path ) )
        {
            ResolveFont( path );
        }
    }


    private void ResolveFont( string exportPath )
    {
        if ( !fontFile ) throw new UnityException( fontFile.name + "is not a valid font-xml file" );

        Font font = new Font();

        XmlDocument xml = new XmlDocument();
        xml.LoadXml( fontFile.text );

        XmlNode info = xml.GetElementsByTagName( "info" )[ 0 ];
        XmlNodeList chars = xml.GetElementsByTagName( "chars" )[ 0 ].ChildNodes;

        CharacterInfo[] charInfos = new CharacterInfo[ chars.Count ];

        for ( int cnt = 0; cnt < chars.Count; cnt++ )
        {
            XmlNode node = chars[ cnt ];
            CharacterInfo charInfo = new CharacterInfo();

            charInfo.index = ToInt( node, "id" );
            charInfo.width = ToInt( node, "xadvance" );
            charInfo.uv = GetUV( node );
            charInfo.vert = GetVert( node );

            charInfos[ cnt ] = charInfo;
        }


        Shader shader = Shader.Find( "Unlit/Transparent" );
        Material material = new Material( shader );
        material.mainTexture = textureFile;
        AssetDatabase.CreateAsset( material, exportPath + ".mat" );


        font.material = material;
        font.name = info.Attributes.GetNamedItem( "face" ).InnerText;
        font.characterInfo = charInfos;
        AssetDatabase.CreateAsset( font, exportPath + ".fontsettings" );
    }


    private Rect GetUV( XmlNode node )
    {
        Rect uv = new Rect();

        uv.x = ToFloat( node, "x" ) / textureFile.width;
        uv.y = ToFloat( node, "y" ) / textureFile.height;
        uv.width = ToFloat( node, "width" ) / textureFile.width;
        uv.height = ToFloat( node, "height" ) / textureFile.height;
        uv.y = 1f - uv.y - uv.height;

        return uv;
    }


    private Rect GetVert( XmlNode node )
    {
        Rect uv = new Rect();

        uv.x = ToFloat( node, "xoffset" );
        uv.y = ToFloat( node, "yoffset" );
        uv.width = ToFloat( node, "width" );
        uv.height = ToFloat( node, "height" );
        uv.y = -uv.y;
        uv.height = -uv.height;

        return uv;
    }


    private int ToInt( XmlNode node, string name )
    {
        return Convert.ToInt32( node.Attributes.GetNamedItem( name ).InnerText );
    }


    private float ToFloat( XmlNode node, string name )
    {
        return ( float )ToInt( node, name );
    }
}