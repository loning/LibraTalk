using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.UI.Xaml.Primitives.Animations
{
    internal class SideDrawerAnimationContext : AnimationContext
    {
        public Storyboard MainContentStoryboard
        {
            get;
            private set;
        }

        public Storyboard MainContentReversedStoryboard
        {
            get;
            private set;
        }

        public Storyboard DrawerStoryboard
        {
            get;
            private set;
        }

        public Storyboard DrawerReversedStoryboard
        {
            get;
            private set;
        }

        public SideDrawerAnimationContext(
            Storyboard mainContentStoryboard, 
            Storyboard mainContentReversedStoryboard, 
            Storyboard drawerStoryboard, 
            Storyboard drawerReversedStoryboard)
        {
            MainContentStoryboard = mainContentStoryboard;
            MainContentReversedStoryboard = mainContentReversedStoryboard;
            DrawerStoryboard = drawerStoryboard;
            DrawerReversedStoryboard = drawerReversedStoryboard;
        }
    }
}