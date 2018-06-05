namespace common.core.codec
{
    /// <summary>
    /// 编码器接口
    /// </summary>
    public interface IEncoder
    {
        byte[] toByte(Msg msg);
        byte[] encode(Msg msg);
    }
}