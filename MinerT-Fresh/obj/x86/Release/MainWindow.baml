   M S B A M L   `   `   `  ���� a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  
 MinerT   MinerT.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     ]themesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero       � Title$  MinerT - V1.7.1��$	��365��$	��890��$	��365��$	��890��$	��365��$	��890��
������� � Icon$* $/MinerT;component/green_creature.ico�� � 	Resources    lvl0  lvl1  lvl2  lvl3  lvl4  lvl5 �� 
TargetType  BtnControlTemplate+   %Microsoft.Windows.Themes.ButtonChrome   
Background   BorderBrush �� IsDefaulted   RenderDefaulted J� IsMouseOver	   RenderMouseOver
   RenderPressed J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName �� Property �� Value�� %�  &	        &	      &	 0     &	 H     &	 `     &	 x     &	 �     ��icons/lvl0.jpg����icons/lvl1.png����icons/lvl2.jpg����icons/lvl3.jpg����icons/lvl4.jpg����icons/lvl5.jpg���� " ��.R�  $��Chrome��# z��# z��# z #	 z #
 z�� . ._��� #��z��#��z��#��z����. # z  P�  �@J�IsKeyboardFocused . . ��� $ Chrome�� �@ RenderDefaulted .  .��� .+��� -   $��MiningTypeLbl��$��Mining Type:��$��28��$��4,46,0,0q�$	��Top=�$
��Left=�$��71���� -   $��MiningLogScroller��$	��215��$
��Left=�$��3,74,0,0q�$	��Top=�$	��865��.���� -   $��MiningLogTxt��$	��213��$	��848�� �� 
IsReadOnly . �� -   $��MiningUserLbl��$��Mining User:��$��28��$��Right=�$��	0,9,251,0q�$	��Top=��� -   $��MiningUserTxt��$��23��$��Right=�$��	0,12,12,0q�$	��Top=�$	��233���� -   $��MinigBalanceLblFixed��$��Mining Balance:��$��28��$
��Left=�$��
215,48,0,0q�$	��Top=��� -   $��CheckBalance��$��Check Stats��$��23��$��Right=�$��
0,10,368,0q�$	��Top=�$	��132��
�����F�
�������#��[ �� -   $��MineBtn��
������$��Start Mining��$��23��$
��Left=�$��
221,10,0,0q�$	��Top=�$	��132��
�������#��[ �� -	   $��ScheduleBtn��
����iA�$��Schedule��
�������$��23��$
��Left=�$��	4,295,0,0q�$	��Top=�$	��132��#��[ �� -
   $��MiningRewardsTxtLabel��$��Mining Rewards:��$��28��$
��Left=�$��
499,48,0,0q�$	��Top=��� -   $��MiningRewardImage��$��600,40,0,251q�$
��Fill=�$��icons/lvl0.jpg��$��20��$��20��$
��Left=��� -   $��MiningRewardLabel��$��A basic shovel��$��22��$
��Left=�$��
623,49,0,0q�$	��Top=�$	��141��$��10(��� -   $��SingleGpuModeCheckBox�� d� 	IsChecked$
 TrueY�$	��GPU��$��16��$
��left=�$��	79,52,0,0q�$	��Top=� � ToolTip$@ :Check this if the client crashes. It will not attempt CPU.���� -   $��SingleCpuModeCheckBox��$
 TrueY�$	��CPU��$��16��$��Right=�$��
0,52,697,0q�$	��Top=�$&  Check this if you want CPU only.���� -   $��ReportBugButton��
���**��$��
Report Bug��
�������$��23��$��Right=�$��
0,295,14,0q�$	��Top=�$	��132��#��[ �� -   $��	RankLabel��$��Rank:��$��28��$��Right=�$��	0,48,49,0q�$	��Top=��� -   $��RankLabelValue��$��0 of 0��$��28��$��Right=�$��0,48,3,0q�$	��Top=�$��50���� -   $��label1��$��
Coin Type:��$��28��$
��Left=�$��4,11,0,0q�$	��Top=�$��79���� -   $��CoinType��$��23��$
��Left=�$��	77,10,0,0q�$	��Top=�$	��133��
�������
���   �.@��� $��Most Profitable��
������� �� 
IsSelected . $_ YMines the most profitable coin. 1.5% fees. About 20% more profit then straight Doge Coin.���� -   $��MiningBalancesBox��
�������
����İ�$��23��$
��Left=�$��
308,46,0,0q�$	��Top=�$	��182��.@��� -   $
��DOGE��$��0.0 DOGE��
�������
����İ��� -   $	��BTC��$��0.0 BTC��
�������
����İ� . 