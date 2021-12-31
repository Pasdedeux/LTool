namespace LHotfixProject
{
    public class FairyGUI_AdapterRegister
    {
        public static void RegisterAdaptor(ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
#if FGUI
            domain.RegisterCrossBindingAdaptor( new GLoaderAdapter() );
            domain.RegisterCrossBindingAdaptor( new GLoader3DAdapter() );
            domain.RegisterCrossBindingAdaptor( new WindowAdapter() );
            domain.RegisterCrossBindingAdaptor( new GTreeAdapter() );
            domain.RegisterCrossBindingAdaptor( new GTextInputAdapter() );
            domain.RegisterCrossBindingAdaptor( new GTextFieldAdapter() );
            domain.RegisterCrossBindingAdaptor( new GSliderAdapter() );
            domain.RegisterCrossBindingAdaptor( new GScrollBarAdapter() );
            domain.RegisterCrossBindingAdaptor( new GRootAdapter() );
            domain.RegisterCrossBindingAdaptor( new GRichTextFieldAdapter() );
            domain.RegisterCrossBindingAdaptor( new GProgressBarAdapter() );
            domain.RegisterCrossBindingAdaptor( new GObjectAdapter() );
            domain.RegisterCrossBindingAdaptor( new GMovieClipAdapter() );
            domain.RegisterCrossBindingAdaptor( new GListAdapter() );
            domain.RegisterCrossBindingAdaptor( new GLabelAdapter() );
            domain.RegisterCrossBindingAdaptor( new GImageAdapter() );
            domain.RegisterCrossBindingAdaptor( new GGraphAdapter() );
            domain.RegisterCrossBindingAdaptor( new GGroupAdapter() );
            domain.RegisterCrossBindingAdaptor( new GComboBoxAdapter() );
            domain.RegisterCrossBindingAdaptor( new GComponentAdapter() );
            domain.RegisterCrossBindingAdaptor( new GButtonAdapter() );
#endif
        }
    }
}
