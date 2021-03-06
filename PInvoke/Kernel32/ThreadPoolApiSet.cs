﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Vanara.PInvoke
{
	using PTP_CALLBACK_INSTANCE = IntPtr;

	using PTP_CLEANUP_GROUP = IntPtr;
	using PTP_IO = IntPtr;
	using PTP_POOL = IntPtr;
	using PTP_TIMER = IntPtr;
	using PTP_WAIT = IntPtr;
	using PTP_WORK = IntPtr;

	public static partial class Kernel32
	{
		/// <summary>
		/// Applications implement this callback if they call the SetThreadpoolCallbackCleanupGroup function to specify the callback to use when CloseThreadpoolCleanupGroup is called.
		/// </summary>
		/// <param name="ObjectContext">Optional application-defined data specified during creation of the object.</param>
		/// <param name="CleanupContext">Optional application-defined data specified using CloseThreadpoolCleanupGroupMembers.</param>
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void PTP_CLEANUP_GROUP_CANCEL_CALLBACK(IntPtr ObjectContext, IntPtr CleanupContext);

		/// <summary>
		/// Applications implement this callback if they call the TrySubmitThreadpoolCallback function to start a worker thread.
		/// </summary>
		/// <param name="Instance">A TP_CALLBACK_INSTANCE structure that defines the callback instance. Applications do not modify the members of this structure.</param>
		/// <param name="Context">The application-defined data.</param>
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void PTP_SIMPLE_CALLBACK(PTP_CALLBACK_INSTANCE Instance, IntPtr Context);

		/// <summary>Applications implement this callback if they call the SetThreadpoolTimer function to start a worker thread for the timer object.</summary>
		/// <param name="Instance">A TP_CALLBACK_INSTANCE structure that defines the callback instance. Applications do not modify the members of this structure.</param>
		/// <param name="Context">The application-defined data.</param>
		/// <param name="Timer">A TP_TIMER structure that defines the timer object that generated the callback.</param>
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void PTP_TIMER_CALLBACK(PTP_CALLBACK_INSTANCE Instance, IntPtr Context, PTP_TIMER Timer);

		/// <summary>
		/// Applications implement this callback if they call the SetThreadpoolWait function to start a worker thread for the wait object.
		/// </summary>
		/// <param name="Instance">A TP_CALLBACK_INSTANCE structure that defines the callback instance. Applications do not modify the members of this structure.</param>
		/// <param name="Context">The application-defined data.</param>
		/// <param name="Wait">A TP_WAIT structure that defines the wait object that generated the callback.</param>
		/// <param name="WaitResult">The result of the wait operation. This parameter can be one of the following values from WaitForMultipleObjects: WAIT_OBJECT_0, WAIT_TIMEOUT</param>
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void PTP_WAIT_CALLBACK(PTP_CALLBACK_INSTANCE Instance, IntPtr Context, PTP_WAIT Wait, uint WaitResult);

		/// <summary>
		/// Applications implement this callback if they call the StartThreadpoolIo function to start a worker thread for the I/O completion object.
		/// </summary>
		/// <param name="Instance">A TP_CALLBACK_INSTANCE structure that defines the callback instance. Applications do not modify the members of this structure.</param>
		/// <param name="Context">The application-defined data.</param>
		/// <param name="Overlapped">A pointer to a variable that receives the address of the OVERLAPPED structure that was specified when the completed I/O operation was started.</param>
		/// <param name="IoResult">The result of the I/O operation. If the I/O is successful, this parameter is NO_ERROR. Otherwise, this parameter is one of the system error codes.</param>
		/// <param name="NumberOfBytesTransferred">The number of bytes transferred during the I/O operation that has completed.</param>
		/// <param name="Io">A TP_IO structure that defines the I/O completion object that generated the callback.</param>
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void PTP_WIN32_IO_CALLBACK(PTP_CALLBACK_INSTANCE Instance, IntPtr Context, IntPtr Overlapped, uint IoResult,
			UIntPtr NumberOfBytesTransferred, PTP_IO Io);

		/// <summary>
		/// Applications implement this callback if they call the SubmitThreadpoolWork function to start a worker thread for the work object.
		/// </summary>
		/// <param name="Instance">A TP_CALLBACK_INSTANCE structure that defines the callback instance. Applications do not modify the members of this structure.</param>
		/// <param name="Context">The application-defined data.</param>
		/// <param name="Work">A TP_WORK structure that defines the work object that generated the callback.</param>
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void PTP_WORK_CALLBACK(PTP_CALLBACK_INSTANCE Instance, IntPtr Context, PTP_WORK Work);

		[Flags]
		public enum TP_CALLBACK_ENV_FLAGS
		{
			None = 0,
			LongFunction = 1,
			Persistent = 2,
		}

		/// <summary>The priority for the callback relative to other callbacks in the same thread pool.</summary>
		public enum TP_CALLBACK_PRIORITY
		{
			/// <summary>The callback should run at high priority.</summary>
			TP_CALLBACK_PRIORITY_HIGH,
			/// <summary>The callback should run at normal priority.</summary>
			TP_CALLBACK_PRIORITY_NORMAL,
			/// <summary>The callback should run at low priority.</summary>
			TP_CALLBACK_PRIORITY_LOW,
			/// <summary>The callback is invalid.</summary>
			TP_CALLBACK_PRIORITY_INVALID,
		}

		/// <summary>Indicates that the callback may not return quickly.</summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <returns>
		/// <para>
		/// The function returns TRUE if another thread in the thread pool is available for processing callbacks or the thread pool was able to spin up a new
		/// thread. In this case, the current callback function may use the current thread indefinitely.
		/// </para>
		/// <para>
		/// The function returns FALSE if another thread in the thread pool is not available to process callbacks and the thread pool was not able to spin up a
		/// new thread. The thread pool will attempt to spin up a new thread after a delay, but if the current callback function runs long, the thread pool may
		/// lose efficiency.
		/// </para>
		/// </returns>
		// BOOL WINAPI CallbackMayRunLong( _Inout_ PTP_CALLBACK_INSTANCE pci); https://msdn.microsoft.com/en-us/library/windows/desktop/ms681981(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms681981")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CallbackMayRunLong(PTP_CALLBACK_INSTANCE pci);

		/// <summary>Cancels the notification from the <c>StartThreadpoolIo</c> function.</summary>
		/// <param name="pio">A <c>TP_IO</c> structure that defines the I/O completion object. The <c>CreateThreadpoolIo</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CancelThreadpoolIo( _Inout_ PTP_IO pio); https://msdn.microsoft.com/en-us/library/windows/desktop/ms681983(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms681983")]
		public static extern void CancelThreadpoolIo(PTP_IO pio);

		/// <summary>Closes the specified thread pool.</summary>
		/// <param name="ptpp">A <c>TP_POOL</c> structure that defines the thread pool. The <c>CreateThreadpool</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpool( _Inout_ PTP_POOL ptpp); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682030(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682030")]
		public static extern void CloseThreadpool(PTP_POOL ptpp);

		/// <summary>Closes the specified cleanup group.</summary>
		/// <param name="ptpcg">A <c>TP_CLEANUP_GROUP</c> structure that defines the cleanup group. The <c>CreateThreadpoolCleanupGroup</c> returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpoolCleanupGroup( _Inout_ PTP_CLEANUP_GROUP ptpcg); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682033(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682033")]
		public static extern void CloseThreadpoolCleanupGroup(PTP_CLEANUP_GROUP ptpcg);

		/// <summary>
		/// Releases the members of the specified cleanup group, waits for all callback functions to complete, and optionally cancels any outstanding callback functions.
		/// </summary>
		/// <param name="ptpcg">
		/// A <c>TP_CLEANUP_GROUP</c> structure that defines the cleanup group. The <c>CreateThreadpoolCleanupGroup</c> function returns this structure.
		/// </param>
		/// <param name="fCancelPendingCallbacks">
		/// If this parameter is TRUE, the function cancels outstanding callbacks that have not yet started. If this parameter is FALSE, the function waits for
		/// outstanding callback functions to complete.
		/// </param>
		/// <param name="pvCleanupContext">
		/// The application-defined data to pass to the application's cleanup group callback function. You can specify the callback function when you call <c>SetThreadpoolCallbackCleanupGroup</c>.
		/// </param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpoolCleanupGroupMembers( _Inout_ PTP_CLEANUP_GROUP ptpcg, _In_ BOOL fCancelPendingCallbacks, _Inout_opt_ PVOID
		// pvCleanupContext); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682036(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682036")]
		public static extern void CloseThreadpoolCleanupGroupMembers(PTP_CLEANUP_GROUP ptpcg, [MarshalAs(UnmanagedType.Bool)] bool fCancelPendingCallbacks, IntPtr pvCleanupContext);

		/// <summary>Releases the specified I/O completion object.</summary>
		/// <param name="pio">A <c>TP_IO</c> structure that defines the I/O completion object. The <c>CreateThreadpoolIo</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpoolIo( _Inout_ PTP_IO pio); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682038(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682038")]
		public static extern void CloseThreadpoolIo(PTP_IO pio);

		/// <summary>Releases the specified timer object.</summary>
		/// <param name="pti">A <c>TP_TIMER</c> structure that defines the timer object. The <c>CreateThreadpoolTimer</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpoolTimer( _Inout_ PTP_TIMER pti); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682040(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682040")]
		public static extern void CloseThreadpoolTimer(PTP_TIMER pti);

		/// <summary>Releases the specified wait object.</summary>
		/// <param name="pwa">A <c>TP_WAIT</c> structure that defines the wait object. The <c>CreateThreadpoolWait</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpoolWait( _Inout_ PTP_WAIT pwa); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682042(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682042")]
		public static extern void CloseThreadpoolWait(PTP_WAIT pwa);

		/// <summary>Releases the specified work object.</summary>
		/// <param name="pwk">A <c>TP_WORK</c> structure that defines the work object. The <c>CreateThreadpoolWork</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI CloseThreadpoolWork( _Inout_ PTP_WORK pwk); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682043(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682043")]
		public static extern void CloseThreadpoolWork(PTP_WORK pwk);

		/// <summary>Allocates a new pool of threads to execute callbacks.</summary>
		/// <param name="reserved">This parameter is reserved and must be NULL.</param>
		/// <returns>
		/// <para>
		/// If the function succeeds, it returns a <c>TP_POOL</c> structure representing the newly allocated thread pool. Applications do not modify the members
		/// of this structure.
		/// </para>
		/// <para>If function fails, it returns NULL. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// PTP_POOL WINAPI CreateThreadpool( _Reserved_ PVOID reserved); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682456(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682456")]
		public static extern PTP_POOL CreateThreadpool(IntPtr reserved = default(IntPtr));

		/// <summary>Creates a cleanup group that applications can use to track one or more thread pool callbacks.</summary>
		/// <returns>
		/// <para>
		/// If the function succeeds, it returns a <c>TP_CLEANUP_GROUP</c> structure of the newly allocated cleanup group. Applications do not modify the members
		/// of this structure.
		/// </para>
		/// <para>If function fails, it returns NULL. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// PTP_CLEANUP_GROUP WINAPI CreateThreadpoolCleanupGroup(void); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682462(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682462")]
		public static extern PTP_CLEANUP_GROUP CreateThreadpoolCleanupGroup();

		/// <summary>Creates a new I/O completion object.</summary>
		/// <param name="fl">The file handle to bind to this I/O completion object.</param>
		/// <param name="pfnio">The callback function to be called each time an overlapped I/O operation completes on the file. For details, see <c>IoCompletionCallback</c>.</param>
		/// <param name="pv">Optional application-defined data to pass to the callback function.</param>
		/// <param name="pcbe">
		/// <para>
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the environment in which to execute the callback. The <c>InitializeThreadpoolEnvironment</c>
		/// function returns this structure.
		/// </para>
		/// <para>If this parameter is NULL, the callback executes in the default callback environment. For more information, see <c>InitializeThreadpoolEnvironment</c>.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, it returns a <c>TP_IO</c> structure that defines the I/O object. Applications do not modify the members of this structure.
		/// </para>
		/// <para>If the function fails, it returns NULL. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// PTP_IO WINAPI CreateThreadpoolIo( _In_ HANDLE fl, _In_ PTP_WIN32_IO_CALLBACK pfnio, _Inout_opt_ PVOID pv, _In_opt_ PTP_CALLBACK_ENVIRON pcbe); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682464(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682464")]
		public static extern PTP_IO CreateThreadpoolIo(SafeFileHandle fl, PTP_WIN32_IO_CALLBACK pfnio, IntPtr pv, PTP_CALLBACK_ENVIRON pcbe);

		/// <summary>Creates a new timer object.</summary>
		/// <param name="pfnti">The callback function to call each time the timer object expires. For details, see <c>TimerCallback</c>.</param>
		/// <param name="pv">Optional application-defined data to pass to the callback function.</param>
		/// <param name="pcbe">
		/// <para>
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the environment in which to execute the callback. The <c>InitializeThreadpoolEnvironment</c>
		/// function returns this structure.
		/// </para>
		/// <para>If this parameter is NULL, the callback executes in the default callback environment. For more information, see <c>InitializeThreadpoolEnvironment</c>.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, it returns a <c>TP_TIMER</c> structure that defines the timer object. Applications do not modify the members of this structure.
		/// </para>
		/// <para>If the function fails, it returns NULL. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// PTP_TIMER WINAPI CreateThreadpoolTimer( _In_ PTP_TIMER_CALLBACK pfnti, _Inout_opt_ PVOID pv, _In_opt_ PTP_CALLBACK_ENVIRON pcbe); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682466(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682466")]
		public static extern PTP_TIMER CreateThreadpoolTimer(PTP_TIMER_CALLBACK pfnti, IntPtr pv, PTP_CALLBACK_ENVIRON pcbe);

		/// <summary>Creates a new wait object.</summary>
		/// <param name="pfnwa">The callback function to call when the wait completes or times out. For details, see <c>WaitCallback</c>.</param>
		/// <param name="pv">Optional application-defined data to pass to the callback function.</param>
		/// <param name="pcbe">
		/// <para>
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the environment in which to execute the callback. The <c>InitializeThreadpoolEnvironment</c>
		/// function returns this structure.
		/// </para>
		/// <para>If this parameter is NULL, the callback executes in the default callback environment. For more information, see <c>InitializeThreadpoolEnvironment</c>.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, it returns a <c>TP_WAIT</c> structure that defines the wait object. Applications do not modify the members of this structure.
		/// </para>
		/// <para>If the function fails, it returns NULL. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// PTP_WAIT WINAPI CreateThreadpoolWait( _In_ PTP_WAIT_CALLBACK pfnwa, _Inout_opt_ PVOID pv, _In_opt_ PTP_CALLBACK_ENVIRON pcbe); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682474(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682474")]
		public static extern PTP_WAIT CreateThreadpoolWait(PTP_WAIT_CALLBACK pfnwa, IntPtr pv, PTP_CALLBACK_ENVIRON pcbe);

		/// <summary>Creates a new work object.</summary>
		/// <param name="pfnwk">
		/// The callback function. A worker thread calls this callback each time you call <c>SubmitThreadpoolWork</c> to post the work object. For details, see <c>WorkCallback</c>.
		/// </param>
		/// <param name="pv">Optional application-defined data to pass to the callback function.</param>
		/// <param name="pcbe">
		/// <para>
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the environment in which to execute the callback. The <c>InitializeThreadpoolEnvironment</c>
		/// function returns this structure.
		/// </para>
		/// <para>If this parameter is NULL, the callback executes in the default callback environment. For more information, see <c>InitializeThreadpoolEnvironment</c>.</para>
		/// </param>
		/// <returns>
		/// <para>
		/// If the function succeeds, it returns a <c>TP_WORK</c> structure that defines the work object. Applications do not modify the members of this structure.
		/// </para>
		/// <para>If the function fails, it returns NULL. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// PTP_WORK WINAPI CreateThreadpoolWork( _In_ PTP_WORK_CALLBACK pfnwk, _Inout_opt_ PVOID pv, _In_opt_ PTP_CALLBACK_ENVIRON pcbe); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682478(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682478")]
		public static extern PTP_WORK CreateThreadpoolWork(PTP_WORK_CALLBACK pfnwk, IntPtr pv, PTP_CALLBACK_ENVIRON pcbe);

		/// <summary>
		/// Removes the association between the currently executing callback function and the object that initiated the callback. The current thread will no
		/// longer count as executing a callback on behalf of the object.
		/// </summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI DisassociateCurrentThreadFromCallback( _Inout_ PTP_CALLBACK_INSTANCE pci); https://msdn.microsoft.com/en-us/library/windows/desktop/ms682581(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms682581")]
		public static extern void DisassociateCurrentThreadFromCallback(PTP_CALLBACK_INSTANCE pci);

		/// <summary>Specifies the DLL that the thread pool will unload when the current callback completes.</summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <param name="mod">A handle to the DLL.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI FreeLibraryWhenCallbackReturns( _Inout_ PTP_CALLBACK_INSTANCE pci, _In_ HMODULE mod); https://msdn.microsoft.com/en-us/library/windows/desktop/ms683154(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms683154")]
		public static extern void FreeLibraryWhenCallbackReturns(PTP_CALLBACK_INSTANCE pci, IntPtr mod);

		/// <summary>Initializes a callback environment.</summary>
		/// <param name="pcbe">A <c>TP_CALLBACK_ENVIRON</c> structure that defines a callback environment.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID InitializeThreadpoolEnvironment( _Out_ PTP_CALLBACK_ENVIRON pcbe); https://msdn.microsoft.com/en-us/library/windows/desktop/ms683486(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "ms683486")]
		public static void InitializeThreadpoolEnvironment(out PTP_CALLBACK_ENVIRON pcbe) { pcbe = new PTP_CALLBACK_ENVIRON(); }

		/// <summary>Determines whether the specified timer object is currently set.</summary>
		/// <param name="pti">A <c>TP_TIMER</c> structure that defines the timer object. The <c>CreateThreadpoolTimer</c> function returns this structure.</param>
		/// <returns>The return value is TRUE if the timer is set; otherwise, the return value is FALSE.</returns>
		// BOOL WINAPI IsThreadpoolTimerSet( _Inout_ PTP_TIMER pti); https://msdn.microsoft.com/en-us/library/windows/desktop/ms684133(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms684133")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsThreadpoolTimerSet(PTP_TIMER pti);

		/// <summary>Specifies the critical section that the thread pool will release when the current callback completes.</summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <param name="pcs">The critical section.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI LeaveCriticalSectionWhenCallbackReturns( _Inout_ PTP_CALLBACK_INSTANCE pci, _Inout_ PCRITICAL_SECTION pcs); https://msdn.microsoft.com/en-us/library/windows/desktop/ms684171(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms684171")]
		public static extern void LeaveCriticalSectionWhenCallbackReturns(PTP_CALLBACK_INSTANCE pci, IntPtr pcs);

		/// <summary>Retrieves the stack reserve and commit sizes for threads in the specified thread pool.</summary>
		/// <param name="ptpp">A <c>TP_POOL</c> structure that specifies the thread pool. The <c>CreateThreadpool</c> function returns this structure.</param>
		/// <param name="ptpsi">A <c>TP_POOL_STACK_INFORMATION</c> structure that receives the stack reserve and commit size.</param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL QueryThreadpoolStackInformation( _In_ PTP_POOL ptpp, _Out_ PTP_POOL_STACK_INFORMATION ptpsi); https://msdn.microsoft.com/en-us/library/windows/desktop/dd405508(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "dd405508")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool QueryThreadpoolStackInformation(PTP_POOL ptpp, out TP_POOL_STACK_INFORMATION ptpsi);

		/// <summary>Specifies the mutex that the thread pool will release when the current callback completes.</summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <param name="mut">A handle to the mutex.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI ReleaseMutexWhenCallbackReturns( _Inout_ PTP_CALLBACK_INSTANCE pci, _In_ HANDLE mut); https://msdn.microsoft.com/en-us/library/windows/desktop/ms685070(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms685070")]
		public static extern void ReleaseMutexWhenCallbackReturns(PTP_CALLBACK_INSTANCE pci, IntPtr mut);

		/// <summary>Specifies the semaphore that the thread pool will release when the current callback completes.</summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <param name="sem">A handle to the semaphore.</param>
		/// <param name="crel">The amount by which to increment the semaphore object's count.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI ReleaseSemaphoreWhenCallbackReturns( _Inout_ PTP_CALLBACK_INSTANCE pci, _In_ HANDLE sem, _In_ DWORD crel); https://msdn.microsoft.com/en-us/library/windows/desktop/ms685073(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms685073")]
		public static extern void ReleaseSemaphoreWhenCallbackReturns(PTP_CALLBACK_INSTANCE pci, IntPtr sem, uint crel);

		/// <summary>Specifies the event that the thread pool will set when the current callback completes.</summary>
		/// <param name="pci">A <c>TP_CALLBACK_INSTANCE</c> structure that defines the callback instance. The structure is passed to the callback function.</param>
		/// <param name="evt">A handle to the event to be set.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI SetEventWhenCallbackReturns( _Inout_ PTP_CALLBACK_INSTANCE pci, _In_ HANDLE evt); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686214(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686214")]
		public static extern void SetEventWhenCallbackReturns(PTP_CALLBACK_INSTANCE pci, IntPtr evt);

		/// <summary>Associates the specified cleanup group with the specified callback environment.</summary>
		/// <param name="pcbe">
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the callback environment. The <c>InitializeThreadpoolEnvironment</c> function returns this structure.
		/// </param>
		/// <param name="ptpcg">
		/// A <c>TP_CLEANUP_GROUP</c> structure that defines the cleanup group. The <c>CreateThreadpoolCleanupGroup</c> function returns this structure.
		/// </param>
		/// <param name="pfng">
		/// The cleanup callback to be called if the cleanup group is canceled before the associated object is released. The function is called when you call <c>CloseThreadpoolCleanupGroupMembers</c>.
		/// </param>
		/// <returns>This function does not return a value.</returns>
		// VOID SetThreadpoolCallbackCleanupGroup( _Inout_ PTP_CALLBACK_ENVIRON pcbe, _In_ PTP_CLEANUP_GROUP ptpcg, _In_opt_ PTP_CLEANUP_GROUP_CANCEL_CALLBACK pfng);
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686255(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "ms686255")]
		public static void SetThreadpoolCallbackCleanupGroup(this PTP_CALLBACK_ENVIRON pcbe, PTP_CLEANUP_GROUP ptpcg, PTP_CLEANUP_GROUP_CANCEL_CALLBACK pfng)
		{
			pcbe.CleanupGroup = ptpcg;
			pcbe.CleanupGroupCancelCallback = pfng;
		}

		/// <summary>Ensures that the specified DLL remains loaded as long as there are outstanding callbacks.</summary>
		/// <param name="pcbe">
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the callback environment. The <c>InitializeThreadpoolEnvironment</c> function returns this structure.
		/// </param>
		/// <param name="mod">A handle to the DLL.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID SetThreadpoolCallbackLibrary( _Inout_ PTP_CALLBACK_ENVIRON pcbe, _In_ PVOID mod);
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686258(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "ms686258")]
		public static void SetThreadpoolCallbackLibrary(this PTP_CALLBACK_ENVIRON pcbe, IntPtr mod) { pcbe.RaceDll = mod; }

		/// <summary>Specifies that the callback should run on a persistent thread.</summary>
		/// <param name="pcbe">
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the callback environment. The <c>InitializeThreadpoolEnvironment</c> function returns this structure.
		/// </param>
		/// <returns>This function does not return a value.</returns>
		// VOID SetThreadpoolCallbackPersistent( _Inout_ PTP_CALLBACK_ENVIRON pcbe);
		// https://msdn.microsoft.com/en-us/library/windows/desktop/dd405518(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "dd405518")]
		public static void SetThreadpoolCallbackPersistent(this PTP_CALLBACK_ENVIRON pcbe) { pcbe.Flags |= TP_CALLBACK_ENV_FLAGS.Persistent; }

		/// <summary>Sets the thread pool to be used when generating callbacks.</summary>
		/// <param name="pcbe">
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the callback environment. The <c>InitializeThreadpoolEnvironment</c> function returns this structure.
		/// </param>
		/// <param name="ptpp">A <c>TP_POOL</c> structure that defines the thread pool. The <c>CreateThreadpool</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID SetThreadpoolCallbackPool( _Inout_ PTP_CALLBACK_ENVIRON pcbe, _In_ PTP_POOL ptpp);
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686261(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "ms686261")]
		public static void SetThreadpoolCallbackPool(this PTP_CALLBACK_ENVIRON pcbe, PTP_POOL ptpp) { pcbe.Pool = ptpp; }

		/// <summary>Specifies the priority of a callback function relative to other work items in the same thread pool.</summary><param name="pcbe">A <c>TP_CALLBACK_ENVIRON</c> structure that defines the callback environment. The <c>InitializeThreadpoolEnvironment</c> function returns this structure.</param><param name="Priority"><para>The priority for the callback relative to other callbacks in the same thread pool. This parameter can be one of the following <c>TP_CALLBACK_PRIORITY</c> enumeration values:</para><para><list type="table"><listheader><term>Value</term><term>Meaning</term></listheader><item><term>TP_CALLBACK_PRIORITY_HIGH</term><term>The callback should run at high priority. </term></item><item><term>TP_CALLBACK_PRIORITY_LOW</term><term>The callback should run at low priority.</term></item><item><term>TP_CALLBACK_PRIORITY_NORMAL</term><term>The callback should run at normal priority.</term></item></list></para></param><returns>This function does not return a value.</returns>
		// VOID SetThreadpoolCallbackPriority( _Inout_ PTP_CALLBACK_ENVIRON pcbe, _In_ TP_CALLBACK_PRIORITY Priority);
		// https://msdn.microsoft.com/en-us/library/windows/desktop/dd405519(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "dd405519")]
		public static void SetThreadpoolCallbackPriority(this PTP_CALLBACK_ENVIRON pcbe, TP_CALLBACK_PRIORITY Priority) { pcbe.CallbackPriority = Priority; }

		/// <summary>Indicates that callbacks associated with this callback environment may not return quickly.</summary><param name="pcbe">A <c>TP_CALLBACK_ENVIRON</c> structure that defines the callback environment. The <c>InitializeThreadpoolEnvironment</c> function returns this structure.</param><returns>This function does not return a value.</returns>
		// VOID SetThreadpoolCallbackRunsLong( _Inout_ PTP_CALLBACK_ENVIRON pcbe);
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686263(v=vs.85).aspx
		[PInvokeData("WinBase.h", MSDNShortId = "ms686263")]
		public static void SetThreadpoolCallbackRunsLong(this PTP_CALLBACK_ENVIRON pcbe) { pcbe.Flags |= TP_CALLBACK_ENV_FLAGS.LongFunction; }

		/// <summary>
		/// Sets the stack reserve and commit sizes for new threads in the specified thread pool. Stack reserve and commit sizes for existing threads are not changed.
		/// </summary>
		/// <param name="ptpp">A <c>TP_POOL</c> structure that specifies the thread pool. The <c>CreateThreadpool</c> function returns this structure.</param>
		/// <param name="ptpsi">A <c>TP_POOL_STACK_INFORMATION</c> structure that specifies the stack reserve and commit size for threads in the pool.</param>
		/// <returns>
		/// <para>If the function succeeds, the return value is nonzero.</para>
		/// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL SetThreadpoolStackInformation( _Inout_ PTP_POOL ptpp, _In_ PTP_POOL_STACK_INFORMATION ptpsi); https://msdn.microsoft.com/en-us/library/windows/desktop/dd405520(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "dd405520")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadpoolStackInformation(PTP_POOL ptpp, ref TP_POOL_STACK_INFORMATION ptpsi);

		/// <summary>Sets the maximum number of threads that the specified thread pool can allocate to process callbacks.</summary>
		/// <param name="ptpp">A <c>TP_POOL</c> structure that defines the thread pool. The <c>CreateThreadpool</c> function returns this structure.</param>
		/// <param name="cthrdMost">The maximum number of threads.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI SetThreadpoolThreadMaximum( _Inout_ PTP_POOL ptpp, _In_ DWORD cthrdMost); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686266(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686266")]
		public static extern void SetThreadpoolThreadMaximum(PTP_POOL ptpp, uint cthrdMost);

		/// <summary>Sets the minimum number of threads that the specified thread pool must make available to process callbacks.</summary>
		/// <param name="ptpp">A <c>TP_POOL</c> structure that defines the thread pool. The <c>CreateThreadpool</c> function returns this structure.</param>
		/// <param name="cthrdMic">The minimum number of threads.</param>
		/// <returns>
		/// <para>If the function succeeds, it returns TRUE.</para>
		/// <para>If the function fails, it returns FALSE. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI SetThreadpoolThreadMinimum( _Inout_ PTP_POOL ptpp, _In_ DWORD cthrdMic); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686268(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686268")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadpoolThreadMinimum(PTP_POOL ptpp, uint cthrdMic);

		/// <summary>
		/// Sets the timer object—, replacing the previous timer, if any. A worker thread calls the timer object's callback after the specified timeout expires.
		/// </summary>
		/// <param name="pti">
		/// A pointer to a <c>TP_TIMER</c> structure that defines the timer object to set. The <c>CreateThreadpoolTimer</c> function returns this structure.
		/// </param>
		/// <param name="pftDueTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that specifies the absolute or relative time at which the timer should expire. If positive or zero, it
		/// indicates the absolute time since January 1, 1601 (UTC), measured in 100 nanosecond units. If negative, it indicates the amount of time to wait
		/// relative to the current time. For more information about time values, see File Times.
		/// </para>
		/// <para>
		/// If this parameter is NULL, the timer object will cease to queue new callbacks (but callbacks already queued will still occur). Note that if this
		/// parameter is zero, the timer will expire immediately.
		/// </para>
		/// </param>
		/// <param name="msPeriod">
		/// The timer period, in milliseconds. If this parameter is zero, the timer is signaled once. If this parameter is greater than zero, the timer is
		/// periodic. A periodic timer automatically reactivates each time the period elapses, until the timer is canceled.
		/// </param>
		/// <param name="msWindowLength">
		/// The maximum amount of time the system can delay before calling the timer callback. If this parameter is set, the system can batch calls to conserve power.
		/// </param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI SetThreadpoolTimer( _Inout_ PTP_TIMER pti, _In_opt_ PFILETIME pftDueTime, _In_ DWORD msPeriod, _In_opt_ DWORD msWindowLength); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686271(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686271")]
		public static extern void SetThreadpoolTimer(PTP_TIMER pti, ref FILETIME pftDueTime, uint msPeriod, uint msWindowLength);

		/// <summary>
		/// Sets the timer object—, replacing the previous timer, if any. A worker thread calls the timer object's callback after the specified timeout expires.
		/// </summary>
		/// <param name="pti">
		/// A pointer to a <c>TP_TIMER</c> structure that defines the timer object to set. The <c>CreateThreadpoolTimer</c> function returns this structure.
		/// </param>
		/// <param name="pftDueTime">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that specifies the absolute or relative time at which the timer should expire. If positive or zero, it
		/// indicates the absolute time since January 1, 1601 (UTC), measured in 100 nanosecond units. If negative, it indicates the amount of time to wait
		/// relative to the current time. For more information about time values, see File Times.
		/// </para>
		/// <para>
		/// If this parameter is NULL, the timer object will cease to queue new callbacks (but callbacks already queued will still occur). Note that if this
		/// parameter is zero, the timer will expire immediately.
		/// </para>
		/// </param>
		/// <param name="msPeriod">
		/// The timer period, in milliseconds. If this parameter is zero, the timer is signaled once. If this parameter is greater than zero, the timer is
		/// periodic. A periodic timer automatically reactivates each time the period elapses, until the timer is canceled.
		/// </param>
		/// <param name="msWindowLength">
		/// The maximum amount of time the system can delay before calling the timer callback. If this parameter is set, the system can batch calls to conserve power.
		/// </param>
		/// <returns>
		/// If the timer was previously active and was canceled, a value of TRUE is returned. Otherwise a value of FALSE is returned. If FALSE is returned, a
		/// callback may be in progress or about to commence. If this is the case, a subsequent SetThreadpoolTimerEx operation will be properly synchronized with
		/// completion of the timer callback.
		/// </returns>
		// https://msdn.microsoft.com/en-us/library/windows/desktop/dn894018(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("Threadpoolapiset.h", MSDNShortId = "dn894018")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadpoolTimerEx(PTP_TIMER pti, ref FILETIME pftDueTime, uint msPeriod, uint msWindowLength);

		/// <summary>
		/// Sets the wait object—replacing the previous wait object, if any. A worker thread calls the wait object's callback function after the handle becomes
		/// signaled or after the specified timeout expires.
		/// </summary>
		/// <param name="pwa">A pointer to a <c>TP_WAIT</c> structure that defines the wait object. The <c>CreateThreadpoolWait</c> function returns this structure.</param>
		/// <param name="h">
		/// <para>A handle.</para>
		/// <para>If this parameter is NULL, the wait object will cease to queue new callbacks (but callbacks already queued will still occur).</para>
		/// <para>If this parameter is not NULL, it must refer to a valid waitable object.</para>
		/// <para>
		/// If this handle is closed while the wait is still pending, the function's behavior is undefined. If the wait is still pending and the handle must be
		/// closed, use <c>CloseThreadpoolWait</c> to cancel the wait and then close the handle.
		/// </para>
		/// </param>
		/// <param name="pftTimeout">
		/// <para>
		/// A pointer to a <c>FILETIME</c> structure that specifies the absolute or relative time at which the wait operation should time out. If this parameter
		/// points to a positive value, it indicates the absolute time since January 1, 1601 (UTC), in 100-nanosecond intervals. If this parameter points to a
		/// negative value, it indicates the amount of time to wait relative to the current time. For more information about time values, see File Times.
		/// </para>
		/// <para>If this parameter points to 0, the wait times out immediately. If this parameter is NULL, the wait will not time out.</para>
		/// </param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI SetThreadpoolWait( _Inout_ PTP_WAIT pwa, _In_opt_ HANDLE h, _In_opt_ PFILETIME pftTimeout); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686273(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686273")]
		public static extern void SetThreadpoolWait(PTP_WAIT pwa, IntPtr h, ref FILETIME pftTimeout);

		/// <summary>
		/// Sets the wait object—replacing the previous wait object, if any. A worker thread calls the wait object's callback function after the handle becomes
		/// signaled or after the specified timeout expires.
		/// </summary>
		/// <param name="pwa">A pointer to a <c>TP_WAIT</c> structure that defines the wait object. The <c>CreateThreadpoolWait</c> function returns this structure.</param>
		/// <param name="h">
		/// <para>A handle.</para>
		/// <para>If this parameter is NULL, the wait object will cease to queue new callbacks (but callbacks already queued will still occur).</para>
		/// <para>If this parameter is not NULL, it must refer to a valid waitable object.</para>
		/// <para>
		/// If this handle is closed while the wait is still pending, the function's behavior is undefined. If the wait is still pending and the handle must be
		/// closed, use <c>CloseThreadpoolWait</c> to cancel the wait and then close the handle.
		/// </para>
		/// </param>
		/// <param name="pftTimeout">
		/// A pointer to a <c>FILETIME</c> structure that specifies the absolute or relative time at which the wait operation should time out. If this parameter
		/// points to a positive value, it indicates the absolute time since January 1, 1601 (UTC), in 100-nanosecond intervals. If this parameter points to a
		/// negative value, it indicates the amount of time to wait relative to the current time. For more information about time values, see File Times.
		/// </param>
		/// <param name="Reserved">Reserved.</param>
		/// <returns>
		/// TRUE, if the timer was previously active and was canceled; otherwise, FALSE. This return value can be used to maintain reference counts to
		/// synchronize between completion and cancellation of a non-periodic timer operation. If FALSE is returned, a callback may be in progress or about to
		/// commence. If FALSE is returned, a subsequent <c>WaitForThreadpool</c> or <c>WaitForThreadpoolEx</c> Callback operation will complete after that
		/// callback is completed. A subsequent <c>SetThreadpool</c> or <c>SetThreadpoolEx</c> operation that is not later canceled will result in an additional callback.”
		/// </returns>
		// BOOL WINAPI SetThreadpoolWaitEx( _Inout_ PTP_WAIT pwa, _In_opt_ HANDLE h, _In_opt_ PFILETIME pftTimeout, _Reserved_ PVOID Reserved); https://msdn.microsoft.com/en-us/library/windows/desktop/mt186618(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("Threadpoolapiset.h", MSDNShortId = "mt186618")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadpoolWaitEx(PTP_WAIT pwa, IntPtr h, ref FILETIME pftTimeout, IntPtr Reserved);

		/// <summary>
		/// Notifies the thread pool that I/O operations may possibly begin for the specified I/O completion object. A worker thread calls the I/O completion
		/// object's callback function after the operation completes on the file handle bound to this object.
		/// </summary>
		/// <param name="pio">A <c>TP_IO</c> structure that defines the I/O completion object. The <c>CreateThreadpoolIo</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI StartThreadpoolIo( _Inout_ PTP_IO pio); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686326(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686326")]
		public static extern void StartThreadpoolIo(PTP_IO pio);

		/// <summary>Posts a work object to the thread pool. A worker thread calls the work object's callback function.</summary>
		/// <param name="pwk">A <c>TP_WORK</c> structure that defines the work object. The <c>CreateThreadpoolWork</c> function returns this structure.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI SubmitThreadpoolWork( _Inout_ PTP_WORK pwk); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686338(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686338")]
		public static extern void SubmitThreadpoolWork(PTP_WORK pwk);

		/// <summary>Requests that a thread pool worker thread call the specified callback function.</summary>
		/// <param name="pfns">The callback function. For details, see <c>SimpleCallback</c>.</param>
		/// <param name="pv">Optional application-defined data to pass to the callback function.</param>
		/// <param name="pcbe">
		/// <para>
		/// A <c>TP_CALLBACK_ENVIRON</c> structure that defines the environment in which to execute the callback function. The
		/// <c>InitializeThreadpoolEnvironment</c> function returns this structure.
		/// </para>
		/// <para>If this parameter is NULL, the callback executes in the default callback environment. For more information, see <c>InitializeThreadpoolEnvironment</c>.</para>
		/// </param>
		/// <returns>
		/// <para>If the function succeeds, it returns TRUE.</para>
		/// <para>If the function fails, it returns FALSE. To retrieve extended error information, call <c>GetLastError</c>.</para>
		/// </returns>
		// BOOL WINAPI TrySubmitThreadpoolCallback( _In_ PTP_SIMPLE_CALLBACK pfns, _Inout_opt_ PVOID pv, _In_opt_ PTP_CALLBACK_ENVIRON pcbe); https://msdn.microsoft.com/en-us/library/windows/desktop/ms686862(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = true, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms686862")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool TrySubmitThreadpoolCallback(PTP_SIMPLE_CALLBACK pfns, IntPtr pv, PTP_CALLBACK_ENVIRON pcbe);

		/// <summary>Waits for outstanding I/O completion callbacks to complete and optionally cancels pending callbacks that have not yet started to execute.</summary>
		/// <param name="pio">A <c>TP_IO</c> structure that defines the I/O completion object. The <c>CreateThreadpoolIo</c> function returns this structure.</param>
		/// <param name="fCancelPendingCallbacks">Indicates whether to cancel queued callbacks that have not yet started to execute.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI WaitForThreadpoolIoCallbacks( _Inout_ PTP_IO pio, _In_ BOOL fCancelPendingCallbacks); https://msdn.microsoft.com/en-us/library/windows/desktop/ms687038(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms687038")]
		public static extern void WaitForThreadpoolIoCallbacks(PTP_IO pio, [MarshalAs(UnmanagedType.Bool)] bool fCancelPendingCallbacks);

		/// <summary>Waits for outstanding timer callbacks to complete and optionally cancels pending callbacks that have not yet started to execute.</summary>
		/// <param name="pti">
		/// A <c>TP_TIMER</c> structure that defines the timer object. The <c>CreateThreadpoolTimer</c> function returns the <c>TP_TIMER</c> structure.
		/// </param>
		/// <param name="fCancelPendingCallbacks">Indicates whether to cancel queued callbacks that have not yet started to execute.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI WaitForThreadpoolTimerCallbacks( _Inout_ PTP_TIMER pti, _In_ BOOL fCancelPendingCallbacks); https://msdn.microsoft.com/en-us/library/windows/desktop/ms687042(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms687042")]
		public static extern void WaitForThreadpoolTimerCallbacks(PTP_TIMER pti, [MarshalAs(UnmanagedType.Bool)] bool fCancelPendingCallbacks);

		/// <summary>Waits for outstanding wait callbacks to complete and optionally cancels pending callbacks that have not yet started to execute.</summary>
		/// <param name="pwa">
		/// A <c>TP_WAIT</c> structure that defines the wait object. The <c>CreateThreadpoolWait</c> function returns the <c>TP_WAIT</c> structure.
		/// </param>
		/// <param name="fCancelPendingCallbacks">Indicates whether to cancel queued callbacks that have not yet started to execute.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI WaitForThreadpoolWaitCallbacks( _Inout_ PTP_WAIT pwa, _In_ BOOL fCancelPendingCallbacks); https://msdn.microsoft.com/en-us/library/windows/desktop/ms687047(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms687047")]
		public static extern void WaitForThreadpoolWaitCallbacks(PTP_WAIT pwa, [MarshalAs(UnmanagedType.Bool)] bool fCancelPendingCallbacks);

		/// <summary>Waits for outstanding work callbacks to complete and optionally cancels pending callbacks that have not yet started to execute.</summary>
		/// <param name="pwk">A <c>TP_WORK</c> structure that defines the work object. The <c>CreateThreadpoolWork</c> function returns this structure.</param>
		/// <param name="fCancelPendingCallbacks">Indicates whether to cancel queued callbacks that have not yet started to execute.</param>
		/// <returns>This function does not return a value.</returns>
		// VOID WINAPI WaitForThreadpoolWorkCallbacks( _Inout_ PTP_WORK pwk, _In_ BOOL fCancelPendingCallbacks); https://msdn.microsoft.com/en-us/library/windows/desktop/ms687053(v=vs.85).aspx
		[DllImport(Lib.Kernel32, SetLastError = false, ExactSpelling = true)]
		[PInvokeData("WinBase.h", MSDNShortId = "ms687053")]
		public static extern void WaitForThreadpoolWorkCallbacks(PTP_WORK pwk, [MarshalAs(UnmanagedType.Bool)] bool fCancelPendingCallbacks);

		/// <summary>Used to set the stack reserve and commit sizes for new threads in a thread pool.</summary>
		[PInvokeData("threadpoolapiset.h")]
		[StructLayout(LayoutKind.Sequential)]
		public struct TP_POOL_STACK_INFORMATION
		{
			/// <summary>The stack reserve size.</summary>
			public SizeT StackReserve;
			/// <summary>The stack commit size.</summary>
			public SizeT StackCommit;
		}

		/// <summary>Defines a callback environment.</summary>
		[PInvokeData("threadpoolapiset.h")]
		[StructLayout(LayoutKind.Sequential)]
		public class PTP_CALLBACK_ENVIRON
		{
			internal uint Version;
			internal PTP_POOL Pool;
			internal PTP_CLEANUP_GROUP CleanupGroup;
			internal PTP_CLEANUP_GROUP_CANCEL_CALLBACK CleanupGroupCancelCallback;
			internal IntPtr RaceDll;
			internal IntPtr _ActivationContext;
			internal PTP_SIMPLE_CALLBACK _FinalizationCallback;
			internal TP_CALLBACK_ENV_FLAGS Flags;
			internal TP_CALLBACK_PRIORITY CallbackPriority;
			internal uint Size;

			internal PTP_CALLBACK_ENVIRON()
			{
				var isMin7 = Environment.OSVersion.Version >= new Version(6, 1);
				Version = isMin7 ? 3U : 1U;
				if (isMin7)
				{
					CallbackPriority = TP_CALLBACK_PRIORITY.TP_CALLBACK_PRIORITY_NORMAL;
					Size = (uint)Marshal.SizeOf(typeof(PTP_CALLBACK_ENVIRON));
				}
			}

			/// <summary>Indicates a function to call when the callback environment is finalized.</summary>
			/// <value>Pointer to a TP_SIMPLE_CALLBACK structure indicating a function to call when the callback environment is finalized.</value>
			public PTP_SIMPLE_CALLBACK FinalizationCallback { set => _FinalizationCallback = value; }

			/// <summary>Assigns an activation context to the callback environment.</summary>
			/// <value>Pointer to an _ACTIVATION_CONTEXT structure.</value>
			public ActCtxSafeHandle ActivationContext { set => _ActivationContext = value.DangerousGetHandle(); }
		}
	}
}