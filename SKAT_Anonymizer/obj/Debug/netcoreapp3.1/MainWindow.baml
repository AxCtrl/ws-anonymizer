   M S B A M L   `   `   `  ����J  FSKAT_Anonymizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:SKAT_AnonymizerSKAT_Anonymizer  5   	        SKAT_Anonymizer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:SKAT_Anonymizer     5   	     � Title$  (k, t)-Anonymisierer��5   	   #��Z��6&   #��Z��6c    � WindowStartupLocation$ Manual=�5	      
 � Top$ 0��6=    � Left$ 0��6E   .��5
      � $��0,0,0,0q�6    � ColumnDefinitions 5   
   �� 5      $	��92* �6   �� 5      $	��20* �6   �� 5      $��1809* �6   .+�5   
   �� -   5         ��Name$ txtbxFilename��5      $
��Left=�6)   $��	53,56,0,0q�6D    �� TextWrapping$
 Wrap=�6W   $	��Top=�6k   $	��575��6�   $��23��6�   $��3��6�   �� 5   
   -   5         ��Name$ btnOpen��5      $��Datei öffnen��6"   $
��Left=�69   $��
538,58,0,0q�6T   $	��Top=�6h   $��21��6�   $	��182��6�   $��2��6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dgData��5      $
��Left=�6#   $��
53,141,0,0q�6>   $
��1111��6R   	   
IsReadOnly$
	 True��6_   $	��338��6q   $	��Top=�6~   $��3��6�   �� 5   
   -   5        $ btnGenerateData��5      $��Patientendaten generieren��6*   $
��Left=�6N   $��22��6i   $��
538,10,0,0q�6u   $	��Top=�6�   $	��182��6�   $��2��6�   �� 5   
   -   5        $ 
inDataSize��5      $
��Left=�6&   $��22��6A   $��
465,10,0,0q�6M   $
 Wrap=�6a   $	��Top=�6u   $��52��6�   $��2��6�     5   
   -   5        $ dgAnonymousData��5      $
��Left=�6,   $��
56,580,0,0q�6G   $
��1111��6[   $
	 True��6h   $	��362��6z   $	��Top=�6�   $��3��6�     5   
   -   5        $ dgAnonymityMeasure��5      $
��Left=�6/   $��1104,141,0,0q�6J   $	��667��6`   $
	 True��6l   $	��338��6~   $	��Top=�6�   $��2��6�   �� 5   
   -   5        $ btnAnonymize��5      $"��Patientendaten Anonymisieren��6'   $
��Left=�6N   $��538,538,0,0q�6i   $	��Top=�6~   $��20��6�   $	��182��6�   $
��Bold"�6�   $��2��6�   �� 5   
   -	   5        
 ��Name$
 lblPatientData��5      $��Patientendaten: ��6(   $
��Left=�6C   $��
53,103,0,0q�6^   $	��Top=�6r   $��30��6�   $	��287��6�   $
��Bold"�6�   $��14(�6�   $��3��6�   �� 5   
   -
   5        $
 lblAnonymizedData��5      $��Anonymisierte Daten:��6+   $
��Left=�6J   $��
56,533,0,0q�6e   $	��Top=�6y   $��31��6�   $	��547��6�   $
��Bold"�6�   $��14(�6�   $��3��6�   �� 5   
   -   5        $
 
ktMeasures��5      $-��'(k,t)Pro Gruppe und sensitivem Attribut��6$   $
��Left=�6V   $��1104,103,0,0q�6q   $	��Top=�6�   $��30��6�   $	��287��6�   $
��Bold"�6�   $��14(�6�   $��2��6�     5   
   -   5        $ dgMicroAggregatedData��5      $
��Left=�62   $��1107,580,0,0q�6M   $	��667��6c   $
	 True��6o   $	��362��6�   $	��Top=�6�   $��2��6�   �� 5   
   -   5        $
 microAggregatedData��5      $��Mikro-Aggregierte Daten��6-   $
��Left=�6O   $��1107,533,0,0q�6j   $	��Top=�6�   $��30��6�   $	��287��6�   $
��Bold"�6�   $��14(�6�   $��2��6�   �� 5   
   -   5         ��Name$ cbSAGeneralization��5      $*��$Generalisierung sensitiver Attribute��6/   $
��Left=�6^   $��752,551,0,0q�6y   $	��Top=�6�   $��2��6�   �� 5   
   -   5        $ cbLimitValueSuppression��5      $*��$Tupel-Unterdrückung für Grenzwerte��64   $
��Left=�6a   $��752,521,0,0q�6|   $	��Top=�6�   $��2��6�   5      5       