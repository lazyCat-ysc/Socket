   M S B A M L   `   `   `  ����C  ?DBServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ServerServer  5   	   3clr-namespace:Server.ViewModelServer.ViewModel  5   	       Server.View.MainView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Server     5   	   4	ViewModelclr-namespace:Server.ViewModel     5   	   -   5        $��MainWindows��5	   	     � Title$  DBSever��5
   	   $	��450��6   $	��800��6&   .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $
��1.8* �6   .+�5   
   �� -   5         ��Name$ laber��5      $��服务器IP：��6   $
��Left=�60   $��Center=�6K   $��20(�6f   �� 5   
   -   5        $ laber1��5      $
��Left=�6F   $��Center=�6a   $��20(�6|   $��90,0,0,0q�6�   ��6    ��  �� Path$ mainModel.Ip�5      �� 5   
   -   5        $ laber2��5      $��	端口：��6    $
��Left=�6.   $��Center=�6I   $��20(�6d   $��	220,0,0,0q�6r   �� 5   
   -   5        $ laber3��5      $
��Left=�6H   $��Center=�6c   $��20(�6~   $��	270,0,0,0q�6�   ��6    �� $ mainModel.Port�6�   �� 5   
   -   5        $ laber4��5      $��运行状态：��6    $
��Left=�60   $��Center=�6K   $��20(�6f   $��	340,0,0,0q�6t   �� 5   
   -   5        $ laber5��5      
���  ��$
��Left=�6^   $��Center=�6y   $��20(�6�   $��	440,0,0,0q�6�   ��6    �� $ mainModel.LaberText�6�   �� 5   
   -   5         ��Name$ button��5      $��Right=�6O   $��Center=�6k   $	��100��6�   $��40��6�   $��0,0,0,0q�6�   ��6!   �� $ mainModel.ButtonText���6�   �� *startCommand+��6�   ��  �� ElementName$ MainWindows���� 5   
   -	   5        $ button1��5      $��1��6"   $��设置��6/   $
��Left=�6<   $��Center=�6W   $	��100��6r   $��40��6~   $��0,0,0,0q�6�   {�.  6�   �� 5   
   -
   5         ��Name$	 set��5      $��2��6!   $
��Left=�6/   $	��340��6J   $	��Top=�6W   $	��794��6o   $��0,0,0,0q�6{   .+�5      �� -   5        $ laber6��5      $��最大人数：��6$   $
��Left=�65   $	��Top=�6P   $��20(�6h   �� 5      -   5         ��Name$ textBox��5      $��Center=�6'   $
��Left=�6I   $��23��6d    �� TextWrapping$
 Wrap=�6p   $��Center=�6�   $��40��6�   $��-16.4,7,0,5.2q�6�   ��6�   �� $ mainModel.TextBox�6�   5      5      5      