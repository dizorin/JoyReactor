using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JoyReactor.Core;
using JoyReactor.Core.Model;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Inject;
using JoyReactor.WP.Common;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace JoyReactor.WP.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Tag> Tags { get; set; }

        public ObservableCollection<Post> Posts { get; set; }

        public RelayCommand<Tag> OpenTagCommand { get; set; }

        public RelayCommand<Post> OpenPostCommand { get; set; }

        private ITagCollectionModel model = InjectService.Instance.Get<ITagCollectionModel>();
        private IPostCollectionModel posModel = InjectService.Instance.Get<IPostCollectionModel>();

        private Tag currentTag;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Tags = new ObservableCollection<Tag>();
            Posts = new ObservableCollection<Post>();

            if (IsInDesignMode)
            {
                Tags.Add(new Tag { Title = "Tag 1" });
                Tags.Add(new Tag { Title = "Tag 2" });
                Tags.Add(new Tag { Title = "Tag 3" });
            }
            else
            {
                OpenTagCommand = new RelayCommand<Tag>(s =>
                {
                    currentTag = s;
                    Initialize(s);
                });
                OpenPostCommand = new RelayCommand<Post>(s =>
                {
                    var vm = SimpleIoc.Default.GetInstance<PostViewModel>();
                    vm.Initialize(currentTag == null ? ID.REACTOR_GOOD : ID.Parser(currentTag.TagId), s.PostId);
                });
                Initialize(null);
            }
        }

        public async void Initialize(Tag tag)
        {
            try
            {
                if (tag == null)
                {
                    Tags.Clear();
                    var tags = await model.GetMainSubscriptionsAsync();
                    tags.ForEach(Tags.Add);
                }

                Posts.Clear();
                var posts = await posModel.GetPostsAsync(tag == null ? ID.REACTOR_GOOD : ID.Parser(tag.TagId), SyncFlags.First);
                posts.ForEach(Posts.Add);
            }
            catch
            {
                // 
            }
        }
    }
}