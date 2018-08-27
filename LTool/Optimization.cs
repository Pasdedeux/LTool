/****************************************************************
 * 作    者：Derek Liu
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/3/28 17:08:14
 * 当前版本：1.0.0.1
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Derek 2018 All rights reserved
*****************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LTool.Unity
{
    class Optimization:MonoBehaviour
    {
        private void Start()
        {
            CombineMesh();
        }

        private void CombineMesh()
        {
            //获取所有子物体的网格
            MeshFilter[] mfChildren = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[ mfChildren.Length ];

            //过去所有子物体的渲染器和材质
            MeshRenderer[] mrChildren = GetComponentsInChildren<MeshRenderer>();
            Material[] materials = new Material[ mrChildren.Length ];

            //生成新的渲染器和网格组件
            MeshRenderer mrSelf = gameObject.AddComponent<MeshRenderer>();
            MeshFilter mfSelf = gameObject.AddComponent<MeshFilter>();

            //合并子纹理
            Texture2D[] textures = new Texture2D[ mrChildren.Length ];
            for( int i = 0; i < mrChildren.Length; i++ )
            {
                //将父对象自身排除
                if( mrChildren[ i ].transform == transform )
                {
                    continue;
                }
                materials[ i ] = mrChildren[ i ].sharedMaterial;
                Texture2D tx = materials[ i ].GetTexture( "_MainTex" ) as Texture2D;
                Texture2D tx2D = new Texture2D( tx.width , tx.height , TextureFormat.ARGB32 , false );
                tx2D.SetPixels( tx.GetPixels( 0 , 0 , tx.width , tx.height ) );
                tx2D.Apply();
                textures[ i ] = tx2D;
            }

            //生成新的材质
            Material materialNew = new Material( materials[ 0 ].shader );
            materialNew.CopyPropertiesFromMaterial( materials[ 0 ] );
            mrSelf.sharedMaterial = materialNew;

            //设置新材质的主纹理
            Texture2D textureNew = new Texture2D( 1024 , 1024 );
            materialNew.SetTexture( "_MainTex" , textureNew );
            Rect[] rects = textureNew.PackTextures( textures , 10 , 1024 );

            //根据纹理合并的信息刷新子网格UV
            for( int i = 0; i < mfChildren.Length; i++ )
            {
                if( mfChildren[ i ].transform == transform )
                {
                    continue;
                }
                Rect rect = rects[ i ];
                Mesh meshCombine = mfChildren[ i ].mesh;
                Vector2[] uvs = new Vector2[ meshCombine.uv.Length ];
                //把网格的uv根据贴图的rect刷一遍
                for( int j = 0; j < uvs.Length; j++ )
                {
                    //这一部分着重理解
                    uvs[ j ].x = rect.x + meshCombine.uv[ j ].x * rect.width;
                    uvs[ j ].y = rect.y + meshCombine.uv[ j ].y * rect.height;
                }
                meshCombine.uv = uvs;
                combine[ i ].mesh = meshCombine;
                combine[ i ].transform = mfChildren[ i ].transform.localToWorldMatrix;
                mfChildren[ i ].gameObject.SetActive( false );
            }
            //生成新的网格，赋值给新的网格渲染组件
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes( combine , true , true );
            mfSelf.mesh = newMesh;
        }
    }
}
