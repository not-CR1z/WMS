namespace WMS.FrontEnd.Layout
{
    public partial class MainLayout
    {

        private bool _iconMenuActive { get; set; }
        private string? IconMenuCssClass => _iconMenuActive ? "width: 80px;" : "with:300px";

        protected void ToggleIconMenu(bool iconMenuActive)
        {
            _iconMenuActive = iconMenuActive;
        }
    }
}