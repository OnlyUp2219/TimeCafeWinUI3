using System.Collections.Generic;

namespace TimeCafeWinUI3.Models
{
    public class CustomDataObject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageLocation { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }

        public static List<CustomDataObject> GetDataObjects()
        {
            return new List<CustomDataObject>
            {
                new CustomDataObject
                {
                    Title = "First Item",
                    Description = "This is a description for the first item. It contains some interesting details about the item.",
                    ImageLocation = "ms-appx:///Assets/StoreLogo.png",
                    Views = 100,
                    Likes = 50
                },
                new CustomDataObject
                {
                    Title = "Second Item",
                    Description = "Another interesting item with its own unique description and details.",
                    ImageLocation = "ms-appx:///Assets/StoreLogo.png",
                    Views = 200,
                    Likes = 75
                },
                new CustomDataObject
                {
                    Title = "Third Item",
                    Description = "The third item in our collection with its own story to tell.",
                    ImageLocation = "ms-appx:///Assets/StoreLogo.png",
                    Views = 150,
                    Likes = 60
                }
            };
        }
    }
} 