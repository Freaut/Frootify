   M S B A M L   `   `   `  ����  Frootify    Frootify.WPFNotification   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Notification��5   	   $	��350��5   	   $	��130��6    � WindowStyle$
 None=�5   	    � AllowsTransparency$
 True��5   	   
������  � Topmost$
 True��5	   	    � ShowInTaskbar$ False��5
   	    � Triggers 5      :� 5   
    :� RoutedEvent$ Window.Loaded��6   .[�5      �� .��5      �� .��5      W� -   5         �� TargetProperty$ Opacity�5   &    W� From 1.06J   		 W� To	 0.06U   
 g� Duration$
 0:0:1D�6^    g� 	BeginTime	 0:0:36o   6�   5      5      5      .��5      �� ���#121212 �� CornerRadius$ 10��6"   $��0q�64   $��10q�5       �� Padding$ 10q�6    J� Opacity$	 0.9Q�6&    J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         Color$ Black��6      
BlurRadius$ 10Q�6-      ShadowDepth$ 0Q�6=   ._�5   
   ��  �� Orientation$ Vertical=�6   .+�5      �� $ 
Horizontal=�6   .+�5      �� $��40��6   $��40��6$   $ 20��60   $��10q�6B   ._�5      �� $
��Fill=�65   ��6   �� *Image+65   5      �� 5      $ Vertical=�6   $��Center=�64   $	��250��6O   .+�5       �� $
��Bold"�5!       
������$��14(�6J   ��5        �� *Title+5!   J   �� 5"      
�������$��12(�5#   3   $
��Wrap=�6A   ��5"       �� *	Message+5#   A   5$      �� 5%      -   5        $��X��5%      $��30��6%   $��30��60   $��Right=�5&      $	��Top=�65   
������
������ 
�������$��14(�5(   ,   $
��Bold"�6:   5)      5*      �� 5+      $ 
Horizontal=�6   $��Center=�62   $��Center=�6O   $��0,00,0,0q�6j   .+�5,      �� -   5         ��Name$ PreviousButton��5,      $
��Auto��61   $��25��6>   
������ $��0q�6c   $��0,0,2,0q�6w   .��5-      �� -   5         ��Name$ PreviousImage��5-      5.      �� 5/      -   5        $ PlayPauseButton��5/      $
��Auto��62   $��25��6?   
������ $��0q�6d   .��50      �� -   5        $ PlayPauseImage��50      51      �� 52      -   5        $ 
SkipButton��52      $
��Auto��6-   $��25��6:   
������ $��0q�6_   $��2,0,0,0q�6s   .��53      �� -	   5        $ 	SkipImage��53      54      55      56      57      58      