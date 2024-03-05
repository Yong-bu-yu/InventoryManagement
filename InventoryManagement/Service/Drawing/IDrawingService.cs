using CommunityToolkit.Maui.Core;
using InventoryManagement.Views.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.Drawing
{
    internal interface IDrawingService
    {
        string FilePath { get; }
        DrawingPage DrawingPage { get; }
        bool IsEnabled { get; set; }

        Task LoadImage(string fileName);
        Task<bool> TryLoadImage(string fileName);

        Task SaveImage(string fileName);

        void Clear();
    }
}
