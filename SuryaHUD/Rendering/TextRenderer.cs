using Vortice.DirectWrite;
using Vortice.Direct2D1;
using Vortice.Mathematics;

namespace SuryaHUD.Rendering
{
   public class TextRenderer : System.IDisposable
   {
       private IDWriteFactory _dwriteFactory;
       private IDWriteTextFormat _textFormat;
       
       public TextRenderer()
       {
           _dwriteFactory = DWrite.DWriteCreateFactory<IDWriteFactory>(Vortice.DirectWrite.FactoryType.Shared);
           
           _textFormat = _dwriteFactory.CreateTextFormat(
               "Consolas", 
               FontWeight.Bold, 
               Vortice.DirectWrite.FontStyle.Normal, 
               16.0f);
               
           _textFormat.TextAlignment = TextAlignment.Leading;
           _textFormat.ParagraphAlignment = ParagraphAlignment.Center;
       }
       
       public void DrawText(ID2D1RenderTarget target, string text, float x, float y, ID2D1Brush brush)
       {
           var layoutRect = new Rect(x, y, 200, 30);
           target.DrawText(text, _textFormat, layoutRect, brush);
       }
       
       public void Dispose()
       {
           _textFormat?.Dispose();
           _dwriteFactory?.Dispose();
       }
   }
}
