   M S B A M L   `   `   `  ����I  EGDS_SERVER_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	   /clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	       GDS_SERVER_WPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:GDS_SERVER_WPF     5   	   -selfclr-namespace:GDS_SERVER_WPF     5   	   -   5          � Title$  
GDS Server��5	   	   $	��748��6   $
��1024��6)    � WindowStartupLocation$ CenterScreen=�6f    � WindowState$ 	Maximized=�6�    � Icon$ Images/logo_gopas_sk.ico��6�    � 	Resources 5
       �� 
TargetType �� Property �� Value    booleanToVisibilityConverter��%q   '��       &	  J     �� 5   
   " ��6   .�5      ��  � ��$
 Left=�6;   5      �� 5   
   6'   5      .��5      �  � RowDefinitions 5   
   �� 5      $	��20* �6   �� 5      $��1* �6   	 � ColumnDefinitions	 5   
   �� 5      $��* �6   �� 5      $
��1.5* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5        
 ��Name$
 
tabControl��5      $��0��6)   $��5��66   $��Stretch=�6J   $
��auto��6h   $��Stretch=�6v   $
��auto��6�   .@�5      �� $
��Main��6   ��5      �� 5       �� EndPoint$	 0,1&�6*    �� 
StartPoint$	 0,0&�69   .��5       �  � Color$ 	#FFF3F3F3��6'    � Offset$ 0Q�69   � 5!      $ 	#FFEBEBEB��6'   $	 0.5Q�69   � 5"      $ 	#FFDDDDDD��6'   $	 0.5Q�69   � 5#      # � ��6'   $ 1Q�6o   5$      .��5&      �  5'      �� 5(      $��1* �6(   �� 5)      $��5 �6(   �� 5*      $��1* �6(   	 5,      �� 5-      $	��200 �6+   �� 5.      $��5 �6+   �� 5/      $��* �6+   ��51      �� 52       �� Color# � ��6*   .+�54      �� $��1��6#   $��3��60   $��5��6D   $��Stretch=�6O   R� 55      -   5         R�Name$ treeViewMachinesAndTasks��55   <   $��0��6   $��0��6,   $��Stretch=�6^   $
��auto��6|   $��Times New Roman)�6�   $��16(�6�   $��Center=�6�   $
��auto��6�   $��Stretch=�6�    56       �� Orientation��%:  'Q�       �� 57      " Q�6$   .�58   "   ��  � �� 59   &   �� 5:   *   .R�5;   .   �� $ 
Horizontal=�69   .+�5<   2   �� $	��img��68   $��30��6C   $��30��6N   $
��Fill=�6Z   $��Images/Folder.ico��6i   �� 5=   2   $	��5,0q�6M   ��6<   �� 6M   5>   /   5?   +   5A   #   5B      5C      5D      �� 5E      $��1��6#   $��5��63   $��Stretch=�6=   �� 5F      -   5         ��Name$ listViewMachineGroups��5F   <   $��0��6   $��3��6,   $��Times New Roman)�6[   $��16(�6x   $	��Top=�6�    � Cursor$  ��6�   $
��auto��6�   $��Stretch=�6�   $��Stretch=�6�   
�������$ �� IsSynchronizedWithCurrentItem$
 TrueY�6~  !   GDS_SERVER_WPF.GridViewSort   AutoSort .  � ContextMenu 5G      �� 5H      .@�5I   "   m� -   5         m�Name$ menuItemRenameWG��5I   +   $��Rename��6E    m� InputGestureText$ F2��6U   6l   m� 5J   "   -   5        $ menuItemDeleteWG��5J   +   $��Delete��6E   $	 DEL��6U   6l   m� 5K   "   -   5        $ menuItemFeaturesWG��5K   +   $��Features��6G   .@�5L   &   m� -   5        $��Wake On Lan��5L   /   $ CTRL+W��6D   6^   m� 5M   &   -	   5        $	��RDP��5M   /   $ CTRL+R��6<   6V   m� 5N   &   -
   5        $��Configure Template��5N   /   $ CTRL+T��6K   6e   5O   #   m� 5P   "   -   5        $��
New Folder��5P   +   $ CTRL+SHIFT+N��6?   6_   5Q       �� View 5S      �� 5T      .P�5U   "   �� $
��Name��61    �� Width$	 180��6?      PropertyName$
 Name��6K    �� CellTemplate 5V   &   �� 5W   *   .R�5X   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5Y   2   ��  ��Name$ imageSource��68   $��20��6M   $��20��6X   ��6d   �� *ImageSource+�� 5Z   2   
���   �$��Center=�6j   ��6<   ��  �� Path$
 Name�6j   5[   /   5\   +   5^   #   �� 5_   "   $��Mac Address��61   $	 150��6F   $ 
MacAddress��6R    5`   &   �� 5a   *   .R�5b   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5c   2   �� 
���   �$��Center=�6p   ��6<   �� $ 
MacAddress�6p   5d   /   5e   +   5g   #   �� 5h   "   $��
IP Address��61   $	 150��6E   $ 	IPAddress��6Q    5i   &   �� 5j   *   .R�5k   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5l   2   �� 
���   �$��Center=�6o   ��6<   �� $ 	IPAddress�6o   5m   /   5n   +   5p   #   �� 5q   "   $��Real PC Name��61   $	 150��6G   $ 
RealPCName��6T    5r   &   �� 5s   *   .R�5t   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5u   2   �� 
���   �$��Center=�6p   ��6<   �� $ 
RealPCName�6p   5v   /   5w   +   5y   #   �� 5z   "   $��Detail��61   $	 300��6A   $ Detail��6M    5{   &   �� 5|   *   .R�5}   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5~   2   �� 
���   �$��Center=�6l   ��6<   �� $ Detail�6l   5   /   5�   +   5�   #   5�      5�      �� 5�      -   5        $ listViewTasks��5�   <   $��0��6   $��3��6,   $y�Hidden=�6S   $��Times New Roman)�6g   $��16(�6�   $	��Top=�6�   $  ��6�   $
��auto��6�   $��Stretch=�6�   $��Stretch=�6�   $
 TrueY�6g   .  5�      �� 5�      .@�5�   "   m� -   5        $��New Task��5�   +   6=   m� 5�   "   -   5        $ menuItemRenameTask��5�   +   $��Rename��6G   $ F2��6W   6m   m� 5�   "   -   5        $ menuItemDeleteTask��5�   +   $��Delete��6G   $	 DEL��6W   6n   m� 5�   "   -   5        $��
New Folder��5�   +   6?   5�       5�      �� 5�      .P�5�   "   �� $
��Name��61   $	 300��6?   $
 Name��6K    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   $��Stretch=�6m   .+�5�   2   �� $��32��68   $��32��6C   ��6O   �� *ImageSource+�� 5�   2   
���   �$��Center=�6j   ��6<   �� $
 Name�6j   5�   /   5�   +   5�   #   �� 5�   "   $��Last Executed��61   $	 150��6H   $ LastExecuted��6T    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6r   ��6<   �� $ LastExecuted�6r   5�   /   5�   +   5�   #   �� 5�   "   $��Machine Group��61   $	 150��6H   $ MachineGroup��6T    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6r   ��6<   �� $ MachineGroup�6r   5�   /   5�   +   5�   #   �� 5�   "   $��	Computers��61   $	 300��6D   $ TargetComputers��6P    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6u   ��6:   �� *TargetComputers+��5�   6   �� 5�   :   $ White��6J    �� Opacity$ 0Q�6X   ��5�   6   �� 5�   :   .R�5�   >   $�   $� Orientation$  
Horizontal=�6U   5�   ;   5�   3   5�   /   5�   +   5�   #   5�      5�      �� 5�      -   5        $ listViewTasksDetails��5�   @   $��2��6   $��3��6,   $��Times New Roman)�6^   $��16(�6{   $	��Top=�6�   $  ��6�   $��Stretch=�6�   $��Stretch=�6�   $
 TrueY�6%   .  5�      �� 5�      .@�5�   "   m� -   5        $ menuItemDeleteDetails��5�   +   $��Delete��6J   6Z   m� 5�   "   -   5        $ menuItemDeleteDetailsAll��5�   +   $��
Delete All��6M   6a   m� 5�   "   -   5        $ menuItemStopDetails��5�   +   $
��Stop��6H   6V   5�       5�      �� 5�      .P�5�   "   �� $
��Name��61   $	 200��6?   $
 Name��6K    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6j   ��6<   �� $
 Name�6j   5�   /   5�   +   5�   #   �� 5�   "   $��Status��61   $ 50��6A   $ Status��6L    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� $��20��6W   $��20��6b   $��Center=�6n   ��68   �� $ Status�6n   5�   /   5�   +   5�   #   �� 5�   "   $��Started��61   $	 150��6B   $ Started��6N    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6m   ��6<   �� $ Started�6m   5�   /   5�   +   5�   #   �� 5�   "   $��Finished��61   $	 150��6C   $ Finished��6O    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6n   ��6<   �� $ Finished�6n   5�   /   5�   +   5�   #   �� 5�   "   $��Clients��61   $ 50��6B   $ Clients��6M    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6m   ��6<   �� $ Clients�6m   5�   /   5�   +   5�   #   �� 5�   "   $
��Done��61   $ 50��6?   $
 Done��6J    5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�   2   �� 
���   �$��Center=�6j   ��6<   �� $
 Done�6j   5�   /   5�   +   5  #   �� 5  "   $��Failed��61   $ 50��6A   $ Failed��6L    5  &   �� 5  *   .R�5  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5  2   �� 
���   �$��Center=�6l   ��6<   �� $ Failed�6l   5  /   5  +   5
  #   �� 5  "   $��Machine Group��61   $	 150��6H   $ MachineGroup��6T    5  &   �� 5  *   .R�5  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5  2   �� 
���   �$��Center=�6r   ��6<   �� $ MachineGroup�6r   5  /   5  +   5  #   5     5     5     5     �� 5     $��Installation Progress��6   ��5     �� 5     $	 0,1&�6*   $	 0,0&�69   .��5     � $ 	#FFF3F3F3��6'   $ 0Q�69   � 5     $ 	#FFEBEBEB��6'   $	 0.5Q�69   � 5     $ 	#FFDDDDDD��6'   $	 0.5Q�69   � 5     # � ��6'   $ 1Q�6o   5      .��5"     � 
�������	 5#     �� 5$     $��* �6+   �� 5%     $��5 �6+   �� 5&     $��* �6+   �� 5'     $��5 �6+   �� 5(     $��* �6+   .+�5*     �� $��1��6#   $��5��63   $��Stretch=�6=   �� 5+     $��3��6#   $��5��63   $��Stretch=�6=   �� 5,     -   5        $! listViewTaskDetailsProgress��5,  /   $��0��6   $
��auto��6T   $
��auto��6b   $��Stretch=�6o   $��Stretch=�6�   $
 TrueY�6�    .  5-     �� 5.     .P�5/  "   �� $
��Name��61   $	 100��6?   $
 Name��6K    50  &   �� 51  *   .R�52  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�53  2   �� 
���   �$��Center=�6j   ��6<   �� $
 Name�6j   54  /   55  +   57  #   �� 58  "   $��Started��61   $	 120��6B   $ Started��6N    59  &   �� 5:  *   .R�5;  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5<  2   �� 
���   �$��Center=�6m   ��6<   �� $ Started�6m   5=  /   5>  +   5@  #   �� 5A  "   $��Machine Group��61   $	 100��6H   $ MachineGroup��6T    5B  &   �� 5C  *   .R�5D  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5E  2   �� 
���   �$��Center=�6r   ��6<   �� $ MachineGroup�6r   5F  /   5G  +   5I  #   5J     5L     �� 5M     -   5        $' !listViewComputersProgressSelected��5M  /   $��2��6   $
��auto��6Z   $
��auto��6h   $��Stretch=�6u   $��Stretch=�6�   $
 TrueY�6�    .  5N     �� 5O     .P�5P  "   �� $
��Name��61   $	 120��6?   $ ComputerName��6K    5Q  &   �� 5R  *   .R�5S  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5T  2   �� 
���   �$��Center=�6r   ��6<   �� $ ComputerName�6r   5U  /   5V  +   5X  #   �� 5Y  "   $��Status��61   $	 150��6A   $
 Step��6M    5Z  &   �� 5[  *   .R�5\  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5]  2   �� 
���   �$��Center=�6j   ��6<   �� $
 Step�6j   5^  /   5_  +   5a  #   �� 5b  "   $
��Time��61   $	 100��6?   $
 Time��6K    5c  &   �� 5d  *   .R�5e  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5f  2   �� 
���   �$��Center=�6j   ��6<   �� $
 Time�6j   5g  /   5h  +   5j  #   5k     5m     �� 5n     -   5        $" listViewComputersProgressAll��5n  /   $��4��6   $
��auto��6U   $
��auto��6c   $��Stretch=�6p   $��Stretch=�6�   $
 TrueY�6�    .  5o     �� 5p     .P�5q  "   �� $
��Name��61   $	 120��6?   $ ComputerName��6K    5r  &   �� 5s  *   .R�5t  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5u  2   �� 
���   �$��Center=�6r   ��6<   �� $ ComputerName�6r   5v  /   5w  +   5y  #   �� 5z  "   $��Status��61   $	 150��6A   $
 Step��6M    5{  &   �� 5|  *   .R�5}  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5~  2   �� 
���   �$��Center=�6j   ��6<   �� $
 Step�6j   5  /   5�  +   5�  #   �� 5�  "   $
��Time��61   $	 100��6?   $
 Time��6K    5�  &   �� 5�  *   .R�5�  .   �� $ 
Horizontal=�69   $��Center=�6R   .+�5�  2   �� 
���   �$��Center=�6j   ��6<   �� $
 Time�6j   5�  /   5�  +   5�  #   5�     5�     5�     5�     �� 5�     $��Console��6   ��5�     �� 5�     $	 0,1&�6*   $	 0,0&�69   .��5�     � $ 	#FFF3F3F3��6'   $ 0Q�69   � 5�     $ 	#FFEBEBEB��6'   $	 0.5Q�69   � 5�     $ 	#FFDDDDDD��6'   $	 0.5Q�69   � 5�     $ White��6'   $ 1Q�65   5�     .��5�     �� -   5        ! ��Name$! 
consoleGDS��5�     $	��Top=�6.   ���	#FFD9F1FF5�     5�     5�     �� 5�  
   -   5        " ��Name$" labelSelected��5�     $��1��6'   $��0��64   $��Selected: 999��6D   $
��Left=�6\   $��Center=�6w   $��20(�6�   $
��Bold"�6�   �� 5�  
   -   5        $" labelFolderContains��5�     $��1��6-   $��1��6:   $��Folder Contains: 999��6J   $
��Left=�6i   $��Center=�6�   $��20(�6�   $
��Bold"�6�   �� 5�  
   -   5        $" labelOnline��5�     $��1��6%   $��2��62   $��Online: 999��6B   $
��Left=�6X   $��Center=�6s   $��20(�6�   $
��Bold"�6�   �� 5�  
   -   5        $" labelOffilne��5�     $��1��6&   $��3��63   $��Offline: 999��6C   $
��Left=�6Z   $��Center=�6u   $��20(�6�   $
��Bold"�6�   �� 5�  
   -   5        $" labelClients��5�     $��1��6&   $��4��63   $��Clients: 999��6C   $
��Left=�6Z   $��20(�6u   $��Center=�6�   $
��Bold"�6�   �� 5�  
   -   5        # ��Name$# buttonClear��5�     $��Clear��6&   $
��Left=�66   $��	183,0,0,0q�6Q   $	��Top=�6d   $��75��6|   $��1��6�   6�   5�     5�     