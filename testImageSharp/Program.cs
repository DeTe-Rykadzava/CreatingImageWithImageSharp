// See https://aka.ms/new-console-template for more information

using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

const int UserAvatarRadius = 128;
const int MarginBeetweenLineAndPhoto = 20;
const float LineThikness = 2.5f;

using SixLabors.ImageSharp.Image welcomeImage = SixLabors.ImageSharp.Image.Load("Assets/WelcomeBg.jpg");

// other options

DrawingOptions overBgOptions = new()
{
    GraphicsOptions = new()
    {
        ColorBlendingMode  = PixelColorBlendingMode.Add
    },
    
};

// black backgroud

DrawingOptions bgBlockDrawOptions = new()
{
    GraphicsOptions = new()
    {
        ColorBlendingMode  = PixelColorBlendingMode.Multiply
    }
};

Brush bg = new SolidBrush(Color.ParseHex("#28282969"));

Polygon bgBlock = new Polygon(new PointF[]
{
    new(20, 20),
    new(welcomeImage.Width - 20, 20),
    new(welcomeImage.Width - 20, welcomeImage.Height - 20),
    new(20, welcomeImage.Height - 20)
});

// user Icon

var userIconResponseMessage = await (new HttpClient()).GetAsync($"https://cdn.discordapp.com/avatars/884724431052742748/eb0d6d31692fab961b7b501a12c343e2.png?size={UserAvatarRadius * 4}");
var userIcon = await userIconResponseMessage.Content.ReadAsStreamAsync();

using var userAvatarImage = await Image.LoadAsync(userIcon);
userAvatarImage.Mutate(x => x.Resize(UserAvatarRadius * 2, UserAvatarRadius * 2));
userAvatarImage.Save("userAvatarTest.jpg");

Brush userIconBrush = new ImageBrush(userAvatarImage);

EllipsePolygon userPolygon = new EllipsePolygon(welcomeImage.Width / 4, welcomeImage.Height / 2, UserAvatarRadius); 

// welcome text

Font welcomeFont = new Font(SystemFonts.Families.FirstOrDefault(x => x.Name == "Arial"), 38, FontStyle.Italic); // See our Fonts library for best practices on retrieving one of these.
string welcomeText = "Welcome";

Font serverFont = new Font(SystemFonts.Families.FirstOrDefault(x => x.Name == "Arial"), 26, FontStyle.Italic); // See our Fonts library for best practices on retrieving one of these.
string serverText = "to the sleepy ash server";

// line under photo
Pen linePen = new SolidPen(Color.White, LineThikness);
PointF[] linePoints = new[]
{
    new PointF(userPolygon.Bounds.X , userPolygon.Bounds.Y + UserAvatarRadius * 2 + MarginBeetweenLineAndPhoto),
    new PointF(userPolygon.Bounds.X + UserAvatarRadius * 2, userPolygon.Bounds.Y + UserAvatarRadius * 2 + MarginBeetweenLineAndPhoto)
};

// draw on the image
welcomeImage.Mutate(x => x.Fill(bgBlockDrawOptions, bg, bgBlock));
welcomeImage.Mutate(x => x.Fill(overBgOptions, userIconBrush, userPolygon));
welcomeImage.Mutate(x=> x.DrawText(welcomeText, welcomeFont, Color.White, new PointF((welcomeImage.Width * 3 / 5) , welcomeImage.Height / 2 - UserAvatarRadius / 2)));
welcomeImage.Mutate(x=> x.DrawText(serverText, serverFont, Color.White, new PointF(welcomeImage.Width * 3 / 5  - welcomeImage.Width / 5 / 2.5f , welcomeImage.Height / 2 + UserAvatarRadius / 2)));
welcomeImage.Mutate(x => x.DrawLine(linePen, linePoints));

// save image
using var file = File.Create("test.jpg");
await welcomeImage.SaveAsync(file, new JpegEncoder());
Console.WriteLine(file.Length);
