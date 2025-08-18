using StainSaver.Models;

namespace StainSaver.Areas.Driver.Models
{
    public class PickUpPageViewModel
    {
        public IEnumerable<PickUp> PickUps { get; set; }
        public PickUpConfirmViewModel ConfirmViewModel { get; set; }
    }
}
