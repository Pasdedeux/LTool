using common.gameData;
namespace common.net.cookie
{
    public class CookieData
    {
        private BaseDao<Cookie> baseDao;
        private static CookieData inst = new CookieData();
        private CookieData()
        {
            baseDao = new BaseDao<Cookie>(CodeMap.Filed.FIELED_COOKIE);
        }
        public static CookieData GetInstance()
        {
            return inst;
        }
        public Cookie Load()
        {
            return baseDao.Load(typeof(Cookie));
        }
        public void Save(Cookie cookie)
        {
            baseDao.Save(cookie);
        }
        public void Clear()
        {
            baseDao.Clear();
        }
    }
}