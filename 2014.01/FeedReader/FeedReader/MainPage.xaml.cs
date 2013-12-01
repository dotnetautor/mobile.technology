using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using FeedReader.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FeedReader.Resources;

namespace FeedReader {
  public partial class MainPage : PhoneApplicationPage {
    // Constructor
    public MainPage() {
      InitializeComponent();

    }

    private async void btnRefersh_Click(object sender, RoutedEventArgs e) {
      const string addr = "http://dotnetautor.de/GetRssFeed";
      var articles = await UpdateFeed(addr);
      lstItems.ItemsSource =  articles;
    }

    private Task<List<FeedItem>> UpdateFeed(string url) {

      var result = new TaskCompletionSource<List<FeedItem>>(); 

      var client = new WebClient();
      client.DownloadStringAsync(new Uri(url), null);
      client.DownloadStringCompleted += (sender, args) => {
        try {
          var feed = XElement.Parse(args.Result);
          // zerlagen der Element au dem XML mit Hilfe con Linq2XML
          var articles = from item in feed.Descendants("item")
            select new FeedItem() {
              Title = item.Element("title").Value,
              DatePublished = item.Element("pubDate").Value,
              Description = item.Element("description").Value,
              ArticleURL = item.Element("guid").Value
            };
          // übergeben des Ergebnis an die TaskCompletionSource 
          result.TrySetResult(articles.ToList());

        } catch (Exception ex) {
          // übergeben eines möglichen Fehlers an die TaskCompletionSource 
          result.TrySetException(ex);
        }
      }; 

      return result.Task;
    }
  }
}