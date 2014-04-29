/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PhoneApp1"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Phone.Controls;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Windows;

namespace PhoneApp1.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ChatViewModel Chat
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChatViewModel>();
            }
        }

        private static string _selectedConversationkey;

        public ConversationViewModel Conversation
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConversationViewModel>(_selectedConversationkey);
            }
        }

        public static void SelectConversation(string key)
        {
            try
            {
                ServiceLocator.Current.GetInstance<ConversationViewModel>(key);
            }
            catch (ActivationException)
            {
                SimpleIoc.Default.Register<ConversationViewModel>(() => (ConversationViewModel)Activator.CreateInstance(typeof(ConversationViewModel), key), key);
            }

            _selectedConversationkey = key;

#if DEBUG
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/ConversationPage.xaml", UriKind.RelativeOrAbsolute));
#endif
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}