   M S B A M L   `   `   `  ����I  EGDS_SERVER_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	   "    GDS_SERVER_WPF.OSAbrivations   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:GDS_SERVER_WPF     5   	   -   5          � Title$  OSAbrivations��5   	   $	��400��6   $	��400��6,    � WindowStartupLocation$ CenterScreen=�6O   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   �� 5      $��4* �6   �� 5      $��1* �6   �� 5      $��1* �6    � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      .+�5   
   �� $��0��6   $��2��6    $��0��64   $��New OS Abrivation:��6A   $
��Left=�6^   $	��Top=�6y   �� 5   
   -   5         ��Name$ textBoxNewAbrivation��5   C   $��0��6   $��2��6"   $��0��66   $��Stretch=�6a   $��Bottom=�6   $
��auto��6�   $
��auto��6�   $��5,5,5,5q�6�   �� 5   
   -   5         ��Name$ 	buttonNew��5   .   $��2��6   $��0��6!   $��Add New��6A   $��75��6S   $
��auto��6^   $��Center=�6l   $��Bottom=�6�   $��5,5,5,5q�6�   6�   �� 5   
   $��3��6   $��0��6(   $��Stretch=�65   $��Bottom=�6S   �� 5   
   -   5         ��Name$ listViewOsAbbrivations��5   5   $��3��6   $��1��6(   $
��auto��6u   $
��auto��6�   $��Stretch=�6�   $��Stretch=�6�   $��5,5,5,5q�6�   ��6U   �� *	MyItems+ � ContextMenu 5      �� 5      .@�5      m� -   5        $��Remove��5       m� Command 6/   �� *
RemoveItem+	 m� CommandParameter	 5      �� 
 �� RelativeSource
 ��  �� AncestorType$ ContextMenuL� �� Path$" PlacementTarget.SelectedItem�6�   5      5       �� 5!   
   $��3��6   $��1��6(   $��Stretch=�65   $��Bottom=�6S   �� 5"   
   $��0��6   $��2��6    $��2��64   $��Selected OS Abrivations:��6A   $
��Left=�6d   $	��Top=�6   �� 5#   
   -   5         ��Name$! listBoxOSAbbrivationsOutPut��5#   3   $��2��6   $��3��6   $
��auto��6X   $��22��6e   $��Bottom=�6q   $��Stretch=�6�   $��Disabled=�6�   $��Disabled=�6�   $��5,5,5,5q�6  ��5$      �� 5%      .R�5&      $�  $� Orientation$ 
Horizontal=�6-   5'      5)      �� 5*   
   $��3��6   $��2��6(   $��Stretch=�65   $��Bottom=�6S   �� 5+   
   -   5        $ buttonOK��5+   .   $��0��6   $��3��6!   $��OK��6@   $��75��6M   $
��auto��6X   $��Center=�6f   $��Center=�6�   $��5,5,5,5q�6�   6�   �� 5,   
   -   5         ��Name$ 
labelError��5,   A   $��0��6   $��3��6    $��3��6-   $y�Hidden=�6U   $��Cannot be selected empty��6i   $��Center=�6�   $��Center=�6�   
���  ��$
��Bold"�6�   �� 5-   
   -	   5        $ buttonCancel��5-   .   $��2��6   $��3��6!   $��Cancel��6D   $��75��6U   $
��auto��6`   $��Center=�6n   $��Center=�6�   $��5,5,5,5q�6�   6�   5.      5/      