   M S B A M L   `   `   `  ����I  EGDS_SERVER_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	   !    GDS_SERVER_WPF.BrowseImages   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:GDS_SERVER_WPF     5   	   -   5          � Title$  BrowseImages��5   	   $	��500��6   $	��700��6+    � Icon$ Images/logo_gopas_sk.ico��67    � WindowStartupLocation$ CenterScreen=�6n   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��2* �6   �� 5      $��3* �6   �� 5      $��1* �6    � RowDefinitions 5   
   �� 5      �� 5      .+�5   
   R� -   5         R�Name$ treeView��5   4   $��0��6   $��2��6#   $��Stretch=�6F   $
��auto��6d   $��Stretch=�6r   $
��auto��6�    � 	Resources 5       �� 
TargetType �� Property	 �� Value
 �� Orientation��%:  'Q�       �� 5      " Q�6   .�5      ��  � ��	 5      �� 5      .R�5   "   �� $
 
Horizontal=�6-   .+�5   &   �� $	��img��6,   $��30��67   $��30��6B   $
��Fill=�6N   $��Images/Folder.ico��6]   �� 5   &   $	��5,0q�6A   ��60   �� 6A   5   #   5      5      5       5!      5"      �� 5#   
   -   5         ��Name$ listView��5#      $��1��6%   $��2��65   $��Stretch=�6G   $
��auto��6e   $��Stretch=�6s   $
��auto��6�    �� View 5$      �� 5%      .P�5&      �� $
��Name��6%    �� Width$	 270��63    �� CellTemplate 5'      �� 5(      .R�5)   "   �� $
 
Horizontal=�6-   $��Center=�6F   .+�5*   &   ��  ��Name$ imageSource��6,   $��20��6A   $��20��6L   ��6X   �� *ImageSource+�� 5+   &   
���   �$��Center=�6^   ��60   ��  �� Path$
 Name�6^   5,   #   5-      5/      50      52      �� 53   
   -   5         ��Name$ 	button_OK��53      $��OK��6$   $��2��61   $��0��6A   $��Center=�6N   $��0,10,0,0q�6k   $	��Top=�6}   $��75��6�   6�   �� 54   
   -   5        $ button_Edit��54      $
��Edit��6&   $��2��65   $��0��6E   $��Center=�6R   $��0,40,0,0q�6o   $	��Top=�6�   $��75��6�   6�   �� 55   
   -   5        $ 
button_New��55      $��	New Image��6%   $��2��69   $��0��6I   $��Center=�6V   $��0,70,0,0q�6s   $	��Top=�6�   $��75��6�   6�   �� 56   
   -   5        $ button_New_Folder��56      $��
New Folder��6,   $��2��6A   $��0��6Q   $��Center=�6^   $��	0,100,0,0q�6{   $	��Top=�6�   $��75��6�   6�   �� 57   
   -   5        $ button_Cancel��57      $��Cancel��6(   $��2��69   $��Center=�6I   $��1��6f   $��Bottom=�6s   $��75��6�   $��0,0,0,10q�6�   6�   59      5:      