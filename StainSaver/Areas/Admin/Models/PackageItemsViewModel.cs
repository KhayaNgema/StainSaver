namespace StainSaver.Areas.Admin.Models
{
    public class PackageItemsViewModel
    {
        public int ComplainId { get; set; }

        public List<LostOrFoundDisplayItemViewModel> Items { get; set; } = new List<LostOrFoundDisplayItemViewModel>();


    }
}
