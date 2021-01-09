using GalaSoft.MvvmLight.Command;

namespace Vts.Gui.BlazorHybrid.Input
{
    public static class Commands
    {
        static Commands()
        {
            SetWavelength = new RelayCommand<double>(SetWavelength_Executed);
            UpdateWavelength = new RelayCommand<double>(UpdateWavelength_Executed);
            UpdateOpticalProperties = new RelayCommand<OpticalProperties>(UpdateOpticalProperties_Executed);
        }

        //Spectra view commands
        public static RelayCommand<double> SetWavelength { get; private set; }
        public static RelayCommand<double> UpdateWavelength { get; private set; }
        public static RelayCommand<OpticalProperties> UpdateOpticalProperties { get; private set; }

        private static void SetWavelength_Executed(double wavelength)
        {
            // not implemented
        }
        private static void UpdateWavelength_Executed(double wavelength)
        {
            // not implemented
        }
        private static void UpdateOpticalProperties_Executed(OpticalProperties ops)
        {
            // not implemented
        }
    }
}