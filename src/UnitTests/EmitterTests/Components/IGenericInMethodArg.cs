﻿namespace Compositional.Composer.UnitTests.EmitterTests.Components
{
	public interface IGenericInMethodArg<in T>
	{
		void SomeMethod(T t);
	}
}
