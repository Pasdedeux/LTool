namespace LHotfixProject{
  public class Common_AdapterRegister {
      public static void RegisterAdaptor( ILRuntime.Runtime.Enviorment.AppDomain domain){
        domain.RegisterCrossBindingAdaptor( new QueueAdapter() );
        domain.RegisterCrossBindingAdaptor( new IEqualityComparer_1_Int32Adapter() );
        domain.RegisterCrossBindingAdaptor( new ApplicationExceptionAdapter() );
      }
  }
}
