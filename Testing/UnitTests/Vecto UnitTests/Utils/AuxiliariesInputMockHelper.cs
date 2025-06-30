using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace TUGraz.Vecto.UnitTests.Utils;

public static class AuxiliariesInputMockHelper
{
    public static Mock<IAuxiliariesDeclarationInputData> AddAuxiliary(this Mock<IAuxiliariesDeclarationInputData> mock, IAuxiliaryDeclarationInputData aux)
    {
        var list = mock.Object?.Auxiliaries ?? new List<IAuxiliaryDeclarationInputData>();
        list.Add(aux);
        mock.Setup(aux => aux.Auxiliaries)
            .Returns(list);

        return mock;
    }

    public static Mock<IAuxiliariesDeclarationInputData> AddAuxiliaries(
        this Mock<IAuxiliariesDeclarationInputData> mock, params IAuxiliaryDeclarationInputData[] aux)
    {

        foreach (var auxiliary in aux)
        {
            mock = mock.AddAuxiliary(auxiliary);
        }


        return mock;
    }

    public static Mock<IAuxiliaryDeclarationInputData> SetType(this Mock<IAuxiliaryDeclarationInputData> mock, AuxiliaryType type)
    {
        mock.Setup(aux => aux.Type).Returns(type);
        return mock;
    }

    public static Mock<IAuxiliaryDeclarationInputData> AddTechnology(this Mock<IAuxiliaryDeclarationInputData> mock, string technology)
    {
        var list = mock.Object.Technology ?? new List<string>();
        list.Add(technology);
        mock.Setup(aux => aux.Technology).Returns(list);
        return mock;
    }
}