   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5   	   
 MinerT   MinerT.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ]themesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero     5   	     � Title$  MinerT - V1.7.1��5   	   $	��365��6!   $	��890��6.   $	��365��5   	   $	��890��6   $	��365��5   	   $	��890��6   
������� � Icon$* $/MinerT;component/green_creature.ico��5        � 	Resources 5	         lvl0  lvl1  lvl2  lvl3  lvl4  lvl5 �� 
TargetType  BtnControlTemplate+   %Microsoft.Windows.Themes.ButtonChrome   
Background   BorderBrush �� IsDefaulted   RenderDefaulted J� IsMouseOver	   RenderMouseOver
   RenderPressed J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName �� Property �� Value�� 5
   
   %�  &	        &	 &     &	 L     &	 r     &	 �     &	 �     &	 �     ��5      icons/lvl0.jpg��67   ��5      icons/lvl1.png��67   ��5      icons/lvl2.jpg��67   ��5      icons/lvl3.jpg��67   ��5      icons/lvl4.jpg��67   ��5      icons/lvl5.jpg��67   �� 5      " ��6   .R�5        $��Chrome��6&   # z��64   # z��6^   # z 6�   #	 z 6�   #
 z��6�    . ._�5      �� #��z��6'   #��z��6J   #��z��6�   ��. # z 6�   5       5      P� 5       �@J�IsKeyboardFocused . . �5      �� $ Chrome��6!    �@ RenderDefaulted .  6P   5      5      5      .��5      � .+�5   
   �� -   5        $��MiningTypeLbl��5   E   $��Mining Type:��6   $��28��6'   $��4,46,0,0q�63   $	��Top=�6Z   $
��Left=�6r   $��71��6�   �� 5   
   -   5        $��MiningLogScroller��5   Q   $	��215��6   $
��Left=�6$   $��3,74,0,0q�6?   $	��Top=�6j   $	��865��6�   .��5       �� -   5        $��MiningLogTxt��5    #   $	��213��6   $	��848��67    �� 
IsReadOnly . 6C   5!      �� 5"   
   -   5        $��MiningUserLbl��5"   b   $��Mining User:��6   $��28��6'   $��Right=�63   $��	0,9,251,0q�6O   $	��Top=�6w   �� 5#   
   -   5        $��MiningUserTxt��5#   M   $��23��6   $��Right=�6   $��	0,12,12,0q�6:   $	��Top=�6b   $	��233��6z   �� 5$   
   -   5        $��MinigBalanceLblFixed��5$   e   $��Mining Balance:��6   $��28��6*   $
��Left=�66   $��
215,48,0,0q�6Q   $	��Top=�6�   �� 5&   
   -   5        $��CheckBalance��5*      $��Check Stats��5&      $��23��5'      $��Right=�5(      $��
0,10,368,0q�5)      $	��Top=�5+      $	��132��5,      
�����F�
�������#��[ 50      �� 53   
   -   5        $��MineBtn��58      
������$��Start Mining��54      $��23��55      $
��Left=�56      $��
221,10,0,0q�57      $	��Top=�59      $	��132��5:      
�������#��[ 5=      �� 5@   
   -	   5        $��ScheduleBtn��5F      
����iA�$��Schedule��5A      
�������$��23��5C      $
��Left=�5D      $��	4,295,0,0q�5E      $	��Top=�5G      $	��132��5H      #��[ 5J      �� 5L   
   -
   5        $��MiningRewardsTxtLabel��5L   e   $��Mining Rewards:��6   $��28��6*   $
��Left=�66   $��
499,48,0,0q�6Q   $	��Top=�6�   �� 5M   
   -   5        $��MiningRewardImage��5M   &   $��600,40,0,251q�6   $
��Fill=�6?   $��icons/lvl0.jpg��6N   $��20��6f   $��20��6q   $
��Left=�6}   �� 5N   
   -   5        $��MiningRewardLabel��5N   d   $��A basic shovel��6   $��22��6)   $
��Left=�65   $��
623,49,0,0q�6P   $	��Top=�6}   $	��141��6�   $��10(�6�   �� 5O   
   -   5        $��SingleGpuModeCheckBox��5O   l    d� 	IsChecked$
 TrueY�6   $	��GPU��6$   $��16��62   $
��left=�6>   $��	79,52,0,0q�6Y   $	��Top=�6�    � ToolTip$@ :Check this if the client crashes. It will not attempt CPU.��6�   6  �� 5P   
   -   5        $��SingleCpuModeCheckBox��5P   n   $
 TrueY�6   $	��CPU��6$   $��16��62   $��Right=�6>   $��
0,52,697,0q�6Z   $	��Top=�6�   $&  Check this if you want CPU only.��6�   6�   �� 5S   
   -   5        $��ReportBugButton��5Y      
���**��$��
Report Bug��5T      
�������$��23��5V      $��Right=�5W      $��
0,295,14,0q�5X      $	��Top=�5Z      $	��132��5[      #��[ 5]      �� 5_   
   -   5        $��	RankLabel��5_   [   $��Rank:��6   $��28��6    $��Right=�6,   $��	0,48,49,0q�6H   $	��Top=�6l   �� 5`   
   -   5        $��RankLabelValue��5`   [   $��0 of 0��6   $��28��6!   $��Right=�6-   $��0,48,3,0q�6I   $	��Top=�6q   $��50��6�   �� 5a   
   -   5        $��label1��5a   ^   $��
Coin Type:��6   $��28��6%   $
��Left=�61   $��4,11,0,0q�6L   $	��Top=�6l   $��79��6�   �� 5b   
   -   5        $��CoinType��5b   M   $��23��6   $
��Left=�6   $��	77,10,0,0q�6:   $	��Top=�6]   $	��133��6u   
�������
���   �.@�5c      �� $��Most Profitable��6   
������� �� 
IsSelected . $_ YMines the most profitable coin. 1.5% fees. About 20% more profit then straight Doge Coin.��6]   5f      �� 5g   
   -   5        $��MiningBalancesBox��5g   �   
�������
����İ�$��23��6G   $
��Left=�6S   $��
308,46,0,0q�6n   $	��Top=�6�   $	��182��6�   .@�5i      �� -   5        $
��DOGE��5i   .   $��0.0 DOGE��6   
�������
����İ�6R   �� 5j      -   5        $	��BTC��5j   -   $��0.0 BTC��6   
�������
����İ� . 6l   5k      5l      5m      