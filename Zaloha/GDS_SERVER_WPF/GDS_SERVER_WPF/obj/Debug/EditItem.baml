   M S B A M L   `   `   `  ����I  EGDS_SERVER_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	       GDS_SERVER_WPF.EditItem   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:GDS_SERVER_WPF     5   	   -   5          � Title$  EditItem��5   	   $	��200��6   $	��400��6'    � WindowStartupLocation$ CenterScreen=�63    � Icon$ Images/logo_gopas_sk.ico��6X    J� IsHitTestVisible . .��5	      �  � RowDefinitions 5
   
   �� 5      �� 5      �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      .+�5   
   �� -   5         ��Name$ labelNewName��5   -   $��0��6   $��0��6    $��	New Name:��6C   $
��Left=�6W   $��Center=�6r   �� 5   
   -   5        $ labelOldNameStatic��5   -   $��1��6   $��0��6    $��	Old Name:��6I   $
��Left=�6]   $��Center=�6x   �� 5   
   -   5        $ labelOldName��5   -   $��1��6   $��0��6    $�� ��6C   $
��Left=�6N   $��Center=�6i   $��70,0,0,0q�6�   �� 5   
   -   5         ��Name$ textBoxNewName��5   C   $��0��6   $��2��6"   $��1��66   $��Stretch=�6[   $��Center=�6y   $��5,5,5,5q�6�   $��1��6�   �� 5   
   -   5        $ 
labelError��5   1   $��2��6   $��2��6$   
���  ��$�� ��6V   $
��Bold"�6a   $��Center=�6s   $��Center=�6�   �� 5   
   -   5         ��Name$ buttonOK��5   .   $��0��6   $��3��6!   $��OK��6@   $��Center=�6M   $��Center=�6j   $��75��6�   6�   �� 5   
   -   5        $ buttonCancel��5   .   $��1��6   $��3��6!   $��Cancel��6D   $��Center=�6U   $��Center=�6r   $��75��6�   6�   5      5      