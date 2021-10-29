using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoBehaviour
    {
        void Start()
        {
            //var ss = GetComponent<LoopScrollRect>();

            //var chapter = Assets.Scripts.Data.ChapterDataModel.Instance;
            //var levelRange = chapter.GetChapterList()[ 2 ].levelsRange;
            //ss.totalCount = ( int )( ( levelRange[ 1 ] - levelRange[ 0 ] + 1 ) * 0.5f );
            //ss.RefillCells();
        }
    }
}