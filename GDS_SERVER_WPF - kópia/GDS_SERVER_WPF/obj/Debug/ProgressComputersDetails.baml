   M S B A M L   `   `   `  ����I  EGDS_SERVER_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	   -    'GDS_SERVER_WPF.ProgressComputersDetails   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:GDS_SERVER_WPF     5   	   -   5          � Title$  ProgressComputersDetails��5   	   $	��500��6*   $	��700��67    � Icon$ Images/logo_gopas_sk.ico��6Z    � WindowStartupLocation$ CenterScreen=�6z   .��5	      � .+�5
   
   �� -   5         ��Name$ listViewProgressDetails��5
      $��Times New Roman)�64   $��16(�6Q    � Cursor$  ��6_   $��Stretch=�6i   $��Stretch=�6�   $��Center=�6�   $��Center=�6�   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�   !   GDS_SERVER_WPF.GridViewSort   AutoSort .  �� View 5      �� 5      .P�5      �� $��Computer Name��6%    �� Width$	 150��6<   	   PropertyName$	 ComputerName��6H   
 �� CellTemplate
 5      �� 5      .R�5   "   ��  �� Orientation$ 
Horizontal=�6-   $��Center=�6F   $��Center=�6a   .+�5   &   �� 
���   �$��Center=�6f   $��Center=�6�   ��60   ��  �� Path$ ComputerName�6�   5   #   5      5      �� 5      $
��Time��6%   $	 150��63   $
	 Time��6?   
 5      �� 5      .R�5   "   �� $ 
Horizontal=�6-   $��Center=�6F   .+�5   &   �� 
���   �$��Center=�6^   ��60   �� $
 Time�6^   5   #   5      5      �� 5      $
��Step��6%   $	 250��63   $
	 Step��6?   
 5       �� 5!      .R�5"   "   �� $ 
Horizontal=�6-   $��Center=�6F   .+�5#   &   �� 
���   �$��Center=�6^   ��60   �� $
 Step�6^   5$   #   5%      5'      �� 5(      $��Status��6%   $ 50��65   $	 Status��6@   
 5)      �� 5*      .R�5+   "   �� $ 
Horizontal=�6-   $��Center=�6F   .+�5,   &   �� $��20��6K   $��20��6V   $��Center=�6b   ��6,   �� $ Status�6b   5-   #   5.      50      51      53      54      55      