/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.Hashing;

namespace TUGraz.VectoHashing
{
	/// <summary>
	/// Interface definition for hashing and verifying VECTO XML files
	/// </summary>
	public interface IVectoHash
	{
		/// <summary>
		/// Get a list of all vecto components contained in the XML file. If a certain
		/// component appears multiple times (e.g. tires) it is provided multiple times 
		/// in the returned list.
		/// to get a list with unique entries (and the number of occurences) use e.g.
		/// GetContainigComponents().GroupBy(s => s).Select(g => new { Entry = g.Key, Count = g.Count() })
		/// </summary>
		/// <returns>List of components contained in the current XML document</returns>
		IList<VectoComponents> GetContainigComponents();


		/// <summary>
		/// Get the digest method used to compute the digest value of the top-level Signature element
		/// if there is no top-level Signature element, the default digest method is returned (see XMLHashProvider.DefaultDigestMethod)
		/// </summary>
		/// <returns>identifier (urn) of the digest method</returns>
		string GetDigestMethod();


		/// <summary>
		/// Get the digest method of the Signature element for the given component. If a component exists 
		/// multiple times (e.g., tires), the index specifies for which component the digest method is returned
		/// </summary>
		/// <param name="component">VectoComponent to get the digest method for</param>
		/// <param name="index">index of the component to use if a component may exist multiple times. Default: 0</param>
		/// <returns>identifier (urn) of the digest method</returns>
		string GetDigestMethod(VectoComponents component, int index = 0);


		/// <summary>
		/// Get the list of canonicalization methods used to compute the digest value of the top-level Signature element
		/// If there is no top-level Signature element, the default digest method is returned (see XMLHashProvider.DefaulCanonicalizationMethod)
		/// </summary>
		/// <returns>returns a list of identifiers (urns) of the canonicalization methods</returns>
		IEnumerable<string> GetCanonicalizationMethods();


		/// <summary>
		/// Get the list of canonicalization methods used to compute the digest value of the Signature element 
		/// for the given component. If a component exists multiple times (e.g., tires) the indes specifies for which
		/// component the canonicalization method is returned
		/// If there is no top-level Signature element, the default digest method is returned (see XMLHashProvider.DefaulCanonicalizationMethod)
		/// </summary>
		/// <param name="component">VectoComponent to get the canonicalization methods for</param>
		/// <param name="index">index of the component to use if a component may exist multiple times. Default: 0</param>
		/// <returns>returns a list of identifiers (urns) of the canonicalization methods</returns>
		IEnumerable<string> GetCanonicalizationMethods(VectoComponents component, int index = 0);


		/// <summary>
		/// Reads the hash-value of the top-level Signature element
		/// </summary>
		/// <returns>base64 encoded hash value</returns>
		string ReadHash();


		/// <summary>
		/// Reads the hash-value of the Signature element for the given component. If a component can exist 
		/// multiple times (i.e., tires), the index specifies for which component the hash is computed
		/// </summary>
		/// <param name="component">VectoComponent to get the canonicalization methods for</param>
		/// <param name="index">index of the component to use if a component may exist multiple times. Default: 0</param>
		/// <returns>base64 encoded hash value</returns>
		string ReadHash(VectoComponents component, int index = 0);


		/// <summary>
		/// Computes the hash-value of the top-level Data element (or vehicle)
		/// If the canoonicalizationMethods is null the canonicalizationMethods from 
		/// the signature element are read if available or the default canonicalization is applied
		/// If the digestMethod is null the digestMethod from the signature element is read if 
		/// available or the default digestMethod is used
		/// Note: the top-level Data element is required to have an id attribute!
		/// </summary>
		/// <param name="canonicalizationMethods">Canonicalization methods to use. If null the default methods are applied</param>
		/// <param name="digestMethod">Digest method to use. If null, the default digest method is used.</param>
		/// <returns>base64 encoded hash value</returns>
		string ComputeHash(IEnumerable<string> canonicalizationMethods = null, string digestMethod = null);


		/// <summary>
		/// Computes the hash-value for the given component. If a component can exist multiple times
		/// (i.e., Tyres) the index specifies for which component the hash is computed
		/// If the canoonicalizationMethods is null the canonicalizationMethods from 
		/// the signature element are read if available or the default canonicalization is applied
		/// If the digestMethod is null the digestMethod from the signature element is read if 
		/// available or the default digestMethod is used
		/// Note: the Data element is required to have an id attribute!
		/// </summary>
		/// <param name="component">VectoComponent to get the canonicalization methods for</param>
		/// <param name="index">index of the component to use if a component may exist multiple times. Default: 0</param>
		/// <param name="canonicalizationMethods">list of identifiers (urn) of the canonicalization methods to apply. If null, the default canonicalization methods are used</param>
		/// <param name="digestMethod">identifier (urn) of the digest method to use. If null, the default digest method is used</param>
		/// <returns></returns>
		string ComputeHash(VectoComponents component, int index = 0, IEnumerable<string> canonicalizationMethods = null,
			string digestMethod = null);


		/// <summary>
		/// Validates the hash of the top-level component (or vehicle)
		/// </summary>
		/// <returns>true, if the re-computed digest value matches the document's digest value, false otherwise</returns>
		bool ValidateHash();


		/// <summary>
		/// Validates the hash for the given component.
		/// </summary>
		/// <param name="component">VectoComponent to get the canonicalization methods for</param>
		/// <param name="index">index of the component to use if a component may exist multiple times. Default: 0</param>
		/// <returns>true, if the re-computed digest value matches the document's digest value, false otherwise</returns>
		bool ValidateHash(VectoComponents component, int index = 0);


		/// <summary>
		/// Computes the hash-value of the outer Data element and adds the according Signature element 
		/// after the Data element.
		/// The default CanocalizationMethods and DigestMethod are used.
		/// Note: the id attribute is added to the Data element automatically. if an id attribute is already
		/// present its value is overwritten unless its lenth is more than 5 characters.
		/// </summary>
		/// <returns>returns the given document including the Signature element with the hash of the Data block</returns>
		XDocument AddHash();
	}
}
