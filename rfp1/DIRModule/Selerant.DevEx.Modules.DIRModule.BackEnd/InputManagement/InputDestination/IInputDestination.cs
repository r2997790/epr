using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
    public interface IInputDestination
    {
        string IdentifiableString { get; }

        decimal InputId { get; }

        decimal OutputCategoryId { get; }

        string MaterialPlant { get; }

        string MaterialCode { get; }

        string ProductSource { get; }

        decimal? Product { get; }

        decimal? Product2 { get; }

        decimal? Product3 { get; }

        decimal? Coproduct { get; }

        decimal? Coproduct2 { get; }

        decimal? FoodRescue { get; }

        decimal? AnimalFeed { get; }

        decimal? BiomassMaterial { get; }

        decimal? CodigestionAnaerobic { get; }

        decimal? Composting { get; }

        decimal? Combustion { get; }

        decimal? LandApplication { get; }

        decimal? Recycling { get; }

        decimal? IncinWithEnRecover { get; }

        decimal? Landfill { get; }

        decimal? NotHarvested { get; }

        decimal? RefuseDiscard { get; }

        decimal? Sewer { get; }

        decimal? EnvironmentLoss { get; }

        decimal? Other { get; }

        DxMaterial Material { get; }
    }
}
