Imports TUGraz.VectoCommon.Utils

Public module SIConvert


    Public sub SIToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, si) = nothing)
            return
        End If

        cevent.Value = CType(cevent.Value, SI).ToGuiFormat()
    End sub

    Public sub TextToSI(Of T As SIBase(of T))(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(T)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().si(of T)
    End sub


    Public sub SIKelvinToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, Kelvin) = nothing)
            return
        End If

        cevent.Value = CType(cevent.Value, Kelvin).AsDegCelsius.ToGuiFormat()
    End sub

    Public sub TextToSIKelvin(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(Kelvin)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().DegCelsiusToKelvin
    End sub


    Public sub SIPerHourToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, PerSecond) = nothing)
            return
        End If

        cevent.Value = (CType(cevent.Value, PerSecond).value() * 3600).ToGuiFormat()
    End sub

    Public sub TextToSIPerHour(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(PerSecond)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().SI(Unit.SI.Per.Hour).cast(Of PerSecond)
    End sub


    Public sub SICubicMeterPerHourToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, CubicMeterPerSecond) = nothing)
            return
        End If

        cevent.Value = (CType(cevent.Value, CubicMeterPerSecond).value() * 3600).ToGuiFormat()
    End sub

    Public sub TextToSICubicMeterPerHour(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(CubicMeterPerSecond)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().SI(Unit.SI.Cubic.Meter.Per.Hour).cast(Of CubicMeterPerSecond)
    End sub


    Public sub SIWattHourPerCubicMeterToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, JoulePerCubicMeter) = nothing)
            return
        End If

        cevent.Value = (CType(cevent.Value, JoulePerCubicMeter).value() / 3600 ).ToGuiFormat()
    End sub

    Public sub TextToSIWattHourPerCubicMeter(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(WattSecondPerCubicMeter)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().SI(Unit.SI.Watt.Hour.Per.Cubic.meter).cast(Of WattSecondPerCubicMeter)
    End sub


    Public sub SIWhkgToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, JoulePerKilogramm) = nothing)
            return
        End If

        cevent.Value = (CType(cevent.Value, JoulePerKilogramm).value() / 3600/ 1000).ToGuiFormat()
    End sub

    Public sub TextToSIkWhkg(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(JoulePerKilogramm)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().SI(Unit.SI.Kilo.Watt.Hour.Per.kilo.Gramm).cast(Of JoulePerKilogramm)
    End sub


    Public sub SIKiloWattToText(sender As object,  cevent As ConvertEventArgs)
        if (cevent.DesiredType <> GetType(string)) 
            return
        End If
        if (TryCast(cevent.Value, Watt) = nothing)
            return
        End If

        cevent.Value = (CType(cevent.Value, Watt).value() / 1000).ToGuiFormat()
    End sub

    Public sub TextToSIKiloWatt(sender As object,  cevent As ConvertEventArgs) 
        if (cevent.DesiredType <> GetType(Watt)) 
            return
        end if

        cevent.Value = cevent.value.tostring().todouble().SI(Unit.SI.Kilo.Watt).cast(Of Watt)
    End sub

End module