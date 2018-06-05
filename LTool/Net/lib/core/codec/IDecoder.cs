namespace common.core.codec
{
    /// <summary>
    /// 解码器接口
    /// </summary>
    public interface IDecoder
    {
        Msg DeCode(byte[] data);
    }
}