   M S B A M L   `   `   `  ����I  EGDS_SERVER_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GDS_SERVER_WPFGDS_SERVER_WPF  5   	   $    GDS_SERVER_WPF.ComputerDetails   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:GDS_SERVER_WPF     5   	   -   5          � Title$  ComputerDetails��5   	   $	��500��6!   $	��500��6.    � Icon$ Images/logo_gopas_sk.ico��6Q    � WindowStartupLocation$ CenterScreen=�6q   .��5	      � .+�5
   
   �� -   5         ��Name$ 
tabControl��5
      $
��Left=�6)   $	��Top=�6K   .@�5      �� $��Computer Details��6   .��5      � ���#FFFFFF.+�5      �� -   5         ��Name$ listBox��5      ��5      �� 5      .R�5   "   � $	��0,2q�6'    � ColumnDefinitions 5   &   �� 5   *   $	��180 �6;   �� 5   *   $��5* �6;   .+�5   &   �� $��0��60   ��6@   �� *Header+�� 5   &   $��1��60   ��6@   �� *	Content+5   #   5      5      5      5      �� 5      $��Configuration��6   .��5      � ���#FFFFFF.+�5      �� -   5         ��Name$ labelComputerName��5      $��ComputerName:��67   $
��Left=�6O   $	��Top=�6j   $��	10,10,0,0q�6�   �� 5       -   5        $ labelWorkgroup��5       $��
Workgroup:��64   $
��Left=�6I   $	��Top=�6d   $��	10,36,0,0q�6|   �� 5!      -   5         ��Name$ textBoxComputerName��5!      $
��Left=�6;   $��23��6V   $��
112,10,0,0q�6b    �� TextWrapping$
 Wrap=�6v   $	��Top=�6�   $	��212��6�   .��6�   ��5"      �� 5#      	 �� 
TargetType"	 ��6$   .�5$   "   -   5        5%      .��5&   )   5'      �� 5(      -   5        $ textBoxWorkGroup��5(      $
��Left=�68   $��23��6S   $��
112,39,0,0q�6_   $
 Wrap=�6s   $	��Top=�6�   $	��212��6�   .��6�   ��5)      �� 5*      "	 ��6$   .�5+   "   -	   5        5,      .��5-   )   5.      �� 5/      -
   5        
 ��Name$
 button��5/      $��Cancel��6-   $��Right=�6>   $��0,0,10.333,9.667q�6Z   $��Bottom=�6t   $	��100��6�   6�   �� 50      -   5        $
 button1��50      $
��Save��6.   $
��Left=�6=   $��10,0,0,9.667q�6X   $��Bottom=�6n   $	��100��6�   6�   51      52      53      55      56      