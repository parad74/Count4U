using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
	[Serializable]
	public class ProcessJob
	{
		public long ID { get; set; }
		public string ProcessJobCode { get; set; }
		public string ProcessCode { get; set; }
		public string SyncCode { get; set; }
		public string PoolCode { get; set; }
		public string ParentProcessCode { get; set; }
		public string FirstProcessCode { get; set; }
		public string NextProcessCode { get; set; }
		public string PrevProcessCode { get; set; }
		public string LastProcessCode { get; set; }
		public string DomainType { get; set; }
		public int NN { get; set; }
		public string Description { get; set; }
		public string JobTypeCode { get; set; }
		public string JobServiceCode { get; set; }
		public string StatusCode { get; set; }

		public DateTime? CreateDate { get; set; }
		public DateTime? GetDate { get; set; }
		public DateTime? ResentDate { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? FinishDate { get; set; }
		public DateTime? ClosedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }

		public string Owner { get; set; }
		public string Device { get; set; }
		public string DbFileName { get; set; }
		public string Tag { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }
		public string TagIP { get; set; }
		public string TagHost { get; set; }
		public string Operation { get; set; }
		public string OperationResult { get; set; }
		public string ContextCBI { get; set; }
		public string CurrentAuditConfigCode { get; set; }
		public string CurrentCBIObjectType { get; set; }
		public string CurrentCBIObjectCode { get; set; }
		public string CurrentCustomerCode { get; set; }
		public string CurrentBranchCode { get; set; }
		public string CurrentInventorCode { get; set; }

		//TODO
		//Order с состоянием 
		// API GateWay - Шлюзы API - это паттерн витрин 
		//Event Bus - шина обмена событиями

		//https://martinfowler.com/bliki/CQRS.html
		//CQRS stands for Command Query Responsibility Segregation
		//CQRS расшифровывается как разделение ответственности по командным запросам. 
		//Это образец, который я впервые услышал, описанный Грегом Янгом.
		//В основе лежит идея о том, что вы можете использовать другую модель для обновления информации, 
		//нежели модель, которую вы используете для чтения информации
		//Изменение, которое вводит CQRS, состоит в том, 
		//чтобы разделить эту концептуальную модель на отдельные модели для обновления и отображения, 
		//которые соответственно называются Command и Query, следуя словарю CommandQuerySeparation. 

		//Мы всегда должны учитывать два разных момента времени: время, когда произошло событие, и время, когда система заметила это событие. (В сложной системе может быть даже несколько замечений.) Эти временные точки тесно соответствуют понятиям фактического и записи, о которых я говорю во временных моделях.
		//Поскольку доменные события являются объектами, их легко записывать. Обычно имеет смысл регистрировать доменные события и вести их учет, если ничего более не делает, это делает хороший контрольный журнал.
		//Еще один стиль сотрудничества - Event Collaboration. В этом стиле у вас никогда не будет одного компонента, который просит другого сделать что-либо, вместо этого каждый компонент сигнализирует о событии, когда что-то меняется. Другие компоненты слушают это событие и реагируют так, как они хотят. Хорошо известный паттерн наблюдателя является примером совместной работы над событиями.
		//Благодаря совместной работе с событиями каждый компонент хранит все необходимые ему данные и прослушивает обновления событий для этих данных. 
		//в Event Collaboration компоненту, отвечающему за обновление некоторых данных, вообще не нужно их хранить, все, что ему нужно сделать, - это обеспечить, чтобы события обновлялись.
		// В результате совместной работы по событию получается очень слабая связь, что делает особенно простым добавление новых компонентов в систему без необходимости изменения существующих компонентов. 
		
		//Событие Sourcing
//Но мы можем пойти дальше, чем контрольный след, на очень интересную территорию. Активатор для этого происходит, когда все изменения в системе вызваны событиями - подход, который я называю Event Sourcing. Другой способ взглянуть на это состоит в том, что Event Sourcing происходит, когда мы можем полностью определить состояние приложения путем обработки журнала событий домена.

		// Handling Events
		//dispatcher 
		//Agreement Dispatcher
		//Одним особенно интересным стилем является использование объекта диспетчера, который запрашивает исходные данные события, чтобы определить, какой фактический объект-исполнитель будет обрабатывать событие. 
		//Это позволяет объектам исполнителя быть простыми, тогда как диспетчер не содержит никакой бизнес-логики, кроме той, которая необходима для поиска правильного исполнителя.
		//Agreement Dispatcher
		//Here the central organizing feature of the dispatch mechanism is a contractual agreement that governs the context of the event. 
		// Здесь центральной организационной особенностью механизма отправки является договорное соглашение, которое регулирует контекст мероприятия.
		//Этот стиль поощряет последовательность делегаций, когда диспетчер опрашивает событие, чтобы найти, в какое соглашение отправлять его, а объект соглашения несет дополнительный набор проверки условий, и цепочка продолжается до тех пор, пока мы не достигнем конечного исполнителя.
		 //Сила в том, что большая часть логики сопоставления исполнителя и события может быть установлена ​​в межобъектных отношениях - фактически в виде данных. Это позволяет системе много настроек

		//A Agreement Dispatcher structures an event processor primarily around the business agreement, since the agreement is a common form of organizing variation in the responses to events.
		//Agreement 
		//Domain Events 
		//Диспетчер соглашений - это способ модуляции процессора для событий домена, чтобы доминирующий модуль был связан с бизнес-соглашением
		//Мое обсуждение диспетчера соглашений следует принципу делегированной отправки. Каждое соглашение вы моделируете как объект, у которого есть метод для обработки события. Фактический код обработки помещается в отдельные объекты процессора, которые загружаются в соглашение структурированным образом. Основное поведение соглашения заключается в том, чтобы найти правильный процессор и затем использовать его для обработки события.
		//Использование соглашения в качестве первого шага в делегировании диспетчеризации облегчает обработку случаев, когда логика обработки событий варьируется в зависимости от действующего бизнес-соглашения, что является распространенным сценарием.

		// Domain Event, Agreement Dispatcher, and Accounting Entry
		// к примеру  Биллинговая система получает различные доменные события и обрабатывает их в учетных записях на счетах клиентов. 

		//Domain Event
		// Здесь у нас есть система с входными данными из пользовательского интерфейса, система обмена сообщениями и некоторые прямые манипуляции с таблицами базы данных. Чтобы разрешить их в доменное событие, у нас есть компоненты, которые взаимодействуют с каждым из этих входных потоков и преобразуют входные данные в поток доменного события, которые хранятся в постоянном журнале. Затем обработчик событий считывает события из журнала и обрабатывает их, заставляя наше приложение делать то, что оно должно делать
		//request driven software speaks when spoken to, event driven software speaks when it has something to say.
		//программное обеспечение, управляемое запросами, говорит, когда с ним говорят, программное обеспечение, управляемое событиями, говорит, когда ему есть, что сказать.
		 //Event Collaboration помогает сохранить каждый компонент простым. Все, что им нужно знать о мире, - это события, которые они слушают.
		// Когда происходит что-то интересное, они испускают событие - им даже не нужно заботиться о том, слушает ли кто-то еще. 
		public ProcessJob()
		{
			ProcessJobCode = "";
			ProcessCode = "";
			SyncCode = "";
			PoolCode = "";
			ParentProcessCode = "";
			FirstProcessCode = "";
			NextProcessCode = "";
			PrevProcessCode = "";
			LastProcessCode = "";
			DomainType = "";
			NN = 0;
			Description = "";
			JobTypeCode = "";
			JobServiceCode = "";
			StatusCode = "";

			Owner = "";
			Device = "";
			DbFileName = "";
			Tag = "";
			Tag1 = "";
			Tag2 = "";
			Tag3 = "";
			TagIP = "";
			TagHost = "";
			Operation = "";
			OperationResult = "";
			ContextCBI = "";
			CurrentAuditConfigCode = "";
			CurrentCBIObjectType = "";
			CurrentCBIObjectCode = "";
			CurrentCustomerCode = "";
			CurrentBranchCode = "";
			CurrentInventorCode = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ProcessJob)) return false;
			return Equals((ProcessJob)obj);
		}

		public bool Equals(ProcessJob other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.ProcessJobCode, this.ProcessJobCode)	);
				//&& Equals(other.AdapterCode, this.AdapterCode)
				//&& Equals(other.FileCode, this.FileCode));
		}

		public override int GetHashCode()
		{
			return (ProcessJobCode != null ? ProcessJobCode.GetHashCode() : 0);
		}


	}
}
