import matrec

mr = matrec.MatrixRows(open(r"D:\\petten\\vecto2\\VECTO\\bin\\Debug\\logs\\MatRec_UrbanDeliveryNoGrad_Time_Gear","br"))

mydataframe= mr.to_df() 

mydataframe
