## Welcome to GitHub Pages

You can use the [editor on GitHub](https://github.com/shmhlove/Asgardium_Client/edit/master/README.md) to maintain and preview the content for your website in Markdown files.

Whenever you commit to this repository, GitHub Pages will run [Jekyll](https://jekyllrb.com/) to rebuild the pages in your site, from the content in your Markdown files.

### Markdown

Markdown is a lightweight and easy-to-use syntax for styling your writing. It includes conventions for

```markdown
Syntax highlighted code block

# Header 1
## Header 2
### Header 3

- Bulleted
- List

1. Numbered
2. List

**Bold** and _Italic_ and `Code` text

[Link](url) and ![Image](src)
```

For more details see [GitHub Flavored Markdown](https://guides.github.com/features/mastering-markdown/).

### Jekyll Themes

Your Pages site will use the layout and styles from the Jekyll theme you have selected in your [repository settings](https://github.com/shmhlove/Asgardium_Client/settings). The name of this theme is saved in the Jekyll `_config.yml` configuration file.

### Support or Contact

Having trouble with Pages? Check out our [documentation](https://help.github.com/categories/github-pages-basics/) or [contact support](https://github.com/contact) and we’ll help you sort it out.

--------------------------------------------------------------------------------------------------------------------

# 아마존 접속
	1. SSH 클라이언트를 개방하십시오. (PuTTY를 사용하여 연결 방법 알아보기)
	2. 프라이빗 키 파일(MangoNight.pem)을 찾습니다. 마법사가 인스턴스를 시작하는 데 사용되는 키를 자동으로 검색합니다.
	3. SSH가 작동하려면 키가 공개적으로 표시되지 않아야 합니다. 필요할 경우 이 명령을 사용합니다.
		chmod 400 MangoNight.pem
	4. 퍼블릭 DNS을(를) 사용하여 인스턴스에 연결:
		ec2-52-79-241-224.ap-northeast-2.compute.amazonaws.com
	예:
		ssh -i "MangoNight.pem" ubuntu@ec2-52-79-241-224.ap-northeast-2.compute.amazonaws.com
		ssh -i "MangoNight.pem" ubuntu@ec2-13-124-43-70.ap-northeast-2.compute.amazonaws.com
		ssh -i "MangoNight.pem" ubuntu@13.124.43.70


	대부분의 경우 위의 사용자 이름이 맞지만, AMI 사용 지침을 숙지하여 AMI 소유자가 기본 AMI 사용자 이름을 변경하지 않도록 하십시오.
	인스턴스에 연결하는 데 도움이 필요한 경우 연결 설명서을(를) 참조하십시오.

--------------------------------------------------------------------------------------------------------------------

# nodemon
재시작 하는게 귀찮다면 nodemon 도구를 사용해서 서버를 실행하면 서버가 수정 될 때마다 자동으로 재시작

npm install -g nodemon
nodemon server.js

--------------------------------------------------------------------------------------------------------------------

# 웹 접속 IP
	http://13.124.43.70:3000

--------------------------------------------------------------------------------------------------------------------

# node 프로세스 관리
	pm2 start ...js	
	pm2 list
	pm2 show 0
	pm2 stop 0

--------------------------------------------------------------------------------------------------------------------

# MongoDB 설치
	/etc/systemd/system/multi-user.target.wants/mongodb.service → /lib/systemd/system/mongodb.service.

--------------------------------------------------------------------------------------------------------------------

# 사용중인 포트 확인 및 프로세스 종료
sudo lsof -i :"포트 번호"
sudo kill -9 "프로세스 번호"

lsof -n -i4TCP:27017
lsof -n -i TCP
fuser -k 27017/tcp 
kill -9 "PID"

--------------------------------------------------------------------------------------------------------------------

# MongoDB
ubuntu@ip-172-31-24-134:~/Asgardium$ sudo service mongodb stop
ubuntu@ip-172-31-24-134:~/Asgardium$ sudo service mongodb start
ubuntu@ip-172-31-24-134:~$ mongo
---
> use Asgardium
> db.Users.insert({username:"test", password:"1234", admin:false})
> db.Users.find()

---
db.getCollection('oracle_Company_AM').insertMany(
   [{ "name": "John", "surname": "Type" },
    { "name": "Master", "surname": "Freez" },
    { "name": "Top", "surname": "Cat" }],
   {
      ordered: false
   }
)
---
collection.update({ name: 'Slime' }, { $set: { hp: 30 } });
collection.findAndModify({ query: { name: 'Demon' }, update: { $set: { att: 150 } }, new: true }); // { 데몬 }
collection.updateOne({ name: 'Slime' }, { $set: { hp: 25 } });
collection.findOneAndUpdate({ name: 'Demon' }, { $set: { att: 150 } }, { returnNewDocument: true });

--------------------------------------------------------------------------------------------------------------------

# 용량 확인
df

- 사용량이 많은 순으로 정렬n
sudo du -ckx | sort -n

--------------------------------------------------------------------------------------------------------------------

#  자동실행 시스템
git 폴링 -> 변경사항체크 후 Pull 및 스크립트 실행 시스템,,, 되려나?? -> contab

--------------------------------------------------------------------------------------------------------------------
# Json 컨트롤
```javascript
object["string"] = "String";
object["name"] = "SangHo";
console.dir(object);
console.log(JSON.stringify(object));
```

```c#
JsonWriter writer = new JsonWriter(json);
writer.PrettyPrint = true; // 한줄로 JsonText를 생성하지않고 사람이 읽기 쉽게 출력
writer.IndentValue = 2; // 들여쓰기
JsonMapper.ToJson(obj, writer);

// JsonUtility.ToJson(class)
// JsonUtility.FromJson<>(jsonString)
//
// if (KeyValuePairs != null)
// {
//     var bodyDataDict = KeyValuePairs.ToDictionary(x => x.Key, x => x.Value);
//     bodyJsonString = LitJson.JsonMapper.ToJson(bodyDataDict);
// }
```
--------------------------------------------------------------------------------------------------------------------
# 예외처리
```javascript
var database = req.app.get('database');

expressApp.on('close', function()
{
	console.log("Express 서버 객체가 종료됩니다.");
	if (databaseModule.db)
    {
		databaseModule.db.close();
	}
});

// 프로세스 종료 시에 데이터베이스 연결 해제
process.on('SIGTERM', function ()
{
    console.log("프로세스가 종료됩니다.");
    app.close();
});

// 에러 핸들러 등록
var errorHandler = expressErrorHandler(
{
    static:
    {
        "404": "./Example/ch06/public/404.html"
    }
});
expressApp.use(expressErrorHandler.httpError(404));
expressApp.use(errorHandler);
```
--------------------------------------------------------------------------------------------------------------------
# VS Code 단축키
### 기본 편집
키|설명|명령ID
------|------|-------
f3|다음 찾기|editor.action.nextMatchFindAction
shift+f3|이전 찾기|editor.action.previousMatchFindAction
alt+Enter|모든 일치 항목을 선택|editor.action.selectAllMatches
ctrl+/|주석 토글|editor.action.commentLine
shift+alt+a|커서위치에 주석 토글|editor.action.blockComment

### 언어 편집
키|설명|명령ID
------|------|-------
f12|정의로 이동|editor.action.goToDeclaration
shift+f12|참조 표시|editor.action.referenceSearch.trigger

### 네비게이션
키|설명|명령ID
------|------|-------
ctrl+p|빠른 열기, 파일로 이동|workbench.action.quickOpen
ctrl+shift+m|오류 보기|workbench.actions.view.problems
alt+왼쪽|뒤로 이동|workbench.action.navigateBack
alt+오른쪽|앞으로 이동|workbench.action.navigateForward
--------------------------------------------------------------------------------------------------------------------
# node-schedule

*    *    *    *    *    *
┬    ┬    ┬    ┬    ┬    ┬
│    │    │    │    │    │
│    │    │    │    │    └ day of week (0 - 7) (0 or 7 is Sun)
│    │    │    │    └───── month (1 - 12)
│    │    │    └────────── day of month (1 - 31)
│    │    └─────────────── hour (0 - 23)
│    └──────────────────── minute (0 - 59)
└───────────────────────── second (0 - 59, OPTIONAL)

## 특수문자 의미
* : 모든 값을 의미한다.
, : 쉼표는 한 항목에 여러 목록을 입력할 때 사용된다. 예를 들어, 5 번째 필드 (요일)에 “MON, WED, FRI”를 사용하면 월요일, 수요일, 금요일을 의미한다.
- : 하이폰은 범위를 정의한다. 예를 들어 2000-2010 년은 2000 년에서 2010 년 사이의 모든 연도를 나타낸다.
% : 명령에서 백분율 기호 (%)는 백 슬래시 ()로 이스케이프하지 않는 한 개행 문자로 변경되고 첫 번째 % 이후의 모든 데이터는 표준 입력으로 명령에 전송된다.

## Example
``` javascript
var schedule = require('node-schedule');
 
var j = schedule.scheduleJob('42 * * * *', function(){
  console.log('The answer to life, the universe, and everything!');
});
```

``` javascript
var j = schedule.scheduleJob('0 17 ? * 0,4-6', function(){
  console.log('Today is recognized by Rebecca Black!');
});
```

``` javascript
var j = schedule.scheduleJob('0 1 * * *', function(fireDate){
  console.log('This job was supposed to run at ' + fireDate + ', but actually ran at ' + new Date());
});
```

``` javascript
var j = schedule.scheduleJob({hour: 14, minute: 30, dayOfWeek: 0}, function(){
  console.log('Time for tea!');
});
```

``` javascript
var schedule = require('node-schedule');
var date = new Date(2012, 11, 21, 5, 30, 0);
 
var j = schedule.scheduleJob(date, function(){
  console.log('The world is going to end today.');
});
```

``` javascript
var schedule = require('node-schedule');
var date = new Date(2012, 11, 21, 5, 30, 0);
var x = 'Tada!';
var j = schedule.scheduleJob(date, function(y){
  console.log(y);
}.bind(null,x));
x = 'Changing Data';
```

``` javascript
var schedule = require('node-schedule');
 
var rule = new schedule.RecurrenceRule();
rule.minute = 42;
 
var j = schedule.scheduleJob(rule, function(){
  console.log('The answer to life, the universe, and everything!');
});
```

``` javascript
var rule = new schedule.RecurrenceRule();
rule.dayOfWeek = [0, new schedule.Range(4, 6)];
rule.hour = 17;
rule.minute = 0;
 
var j = schedule.scheduleJob(rule, function(){
  console.log('Today is recognized by Rebecca Black!');
});
```

``` javascript
let startTime = new Date(Date.now() + 5000);
let endTime = new Date(startTime.getTime() + 5000);
var j = schedule.scheduleJob({ start: startTime, end: endTime, rule: '*/1 * * * * *' }, function(){
  console.log('Time for tea!');
});
```
--------------------------------------------------------------------------------------------------------------------
# 할일
2017-
* ~~라우터로 API를 추가할 수 있는 구조를 만들자.~~
* ~~DB 구조를 만들자.~~
* ~~웹서버, 웹소켓 검증용 클라이언트 구현~~
---
2018-
* ~~레파지토리 분리해야겠다.~~
* ~~동적로드가 필요해지니 클라이언트 프레임워크 구성이 필요해졌다.~~
	* ~~유틸리티::AppInfo~~
	* ~~유틸리티::코루틴~~
	* ~~유틸리티::오브젝트풀~~
	* ~~유틸리티::유틸~~
	* ~~글로벌::경로관리~~
	* ~~글로벌::Enum~~
	* ~~글로벌::Hard~~
	* ~~씬 매니져~~
		* ~~페이드 UI가 잘 동작하는지 확인 하므로써 다듬기 작업 중간체크를 하자.~~
	* ~~UI 매니져~~
		* ~~params 를 이용해서 Show 함수를 특수화 할 수 있도록 구성~~
		* ~~UI 루트와 패널 관리를 Type으로 할것인가? name으로 할것인가?~~
			* ~~name으로 할경우 Get의 오류가 발생했을때 원인을 찾기 힘들 수 있다.~~
			* ~~Type으로 하면 Get을 최대한 컴파일 타임에 오류를 감지할 수 있기에 안정감이 든다.~~
			* ~~name으로 하면 리소스 이름의 자율성이 보장된다.~~
			* ~~Type으로 하면 리소스 이름을 스크립트 이름과 동일시 해야한다.~~
			* ~~Type으로 하면 스크립트를 재활용할 수가 없다.~~
			* ~~Type으로 했을때 재활용단점이 name의 불안정감보다 큰거 같다.. name으로 하자..~~
	* ~~네트워크 매니져~~
	* ~~사운드 매니져~~
	* ~~데이터::리소스~~
		* ~~로드 코드 전부 어싱크로 변경~~
	* ~~데이터::테이블~~
	* ~~데이터::로더~~
	* ~~데이터::리더~~
	* 툴::데이터컨버터
	* 툴::리소스리스팅
	* 툴::빌드스크립트
	* ~~개선::TAP(작업기반비동기)적용~~
		* ~~리소스와 테이블 클래스에 TAP(작업기반비동기) 적용~~
---
* ~~간단한 인증 프로세스를 만들자.~~
	* ~~서버 : 회원가입 및 로그인 처리~~
    * ~~클라 : NGUI로 로그인 클라이언트를 만들자~~
    * ~~클라 : body 데이터를 Json형태로 받아올 수 있도록 파라미터를 구성하자~~
	* ~~클라 : Json데이터를 내려주는 코드로 변경하자.~~
	* ~~클라 : 클라에서는 서버에서 내려준 Json데이터를 파싱하는 코드를 작성하자.~~
	* ~~클라 : 로그인 UI 개발~~
	* ~~클라 : UI 코드 컨트롤 구조가 필요하다.~~
	* ~~클라 : 회원가입 UI 개발~~
	* ~~클라 : 알림팝업 UI 개발~~
	* ~~클라 : 로비씬 진입부 개발~~
	* ~~클라 : PlayerPreb에 이메일과 비밀번호를 저장시켜서 반자동로그인을 시켜주자.~~
---
* ~~로비씬~~
	* ~~기획검토~~
	* ~~클라 : 메인메뉴 개발~~
---
* 업데이트씬
	* ~~클라 : 데이터 로더 점검~~
	* ~~클라 : 테이블은 타이밍 구분없이 무조건 로드~~
	* 클라 : 리소스는 로드 타이밍 구분에 대한 설계
	* 클라 : 애셋번들 패치 설계
	* 클라 : 애셋번들 로드 구조 설계
---
* 마이닝
	* ~~기획검토~~
	* 클라 : UI작업
		* ~~클라 : 그룹버튼을 만들까??, 아니면 그룹토글에 이벤트를 받도록 해서 버튼이든 뭐든 통일화시킬까??~~
		* ~~클라 : 스크롤뷰 동적 추가 작업~~
		* ~~클라 : 스크롤뷰 재사용 적용~~
		* ~~마이닝 파워 카운팅 로직 개발~~
		* ~~기본 마이닝 슬롯 UI 개발~~
	* ~~DB : 서버 테이블 관리구조 설계~~
	* 서버/클라 : 마이닝 비지니스 로직 개발
		* ~~서버/클라 : Config 테이블 적용~~
		* ~~서버/클라 : 기본 마이닝 데이터 테이블 적용~~
		* ~~서버/클라 : 기본 리소스 데이터 테이블 적용~~
		* ~~서버 : 인스턴스 마이닝 액티브 테이블을 구성 (oracle_company_am과 스키마 동일)~~
		* ~~서버 : 인스턴스 마이닝 액티브 테이블 초기설정(UID시스템 + 서버가 켜질때 항상 oracle_company_am을 읽어 추가 혹은 업데이트 해주기)~~
		* ~~서버 : 인스턴스 마이닝 액티브 테이블 Get API 제공~~
		* ~~서버 : 스테틱 테이블 active_mining_quantity 추가 및 Get API 제공~~
		* ~~서버 : 스테틱 테이블 active_mining_supply 추가 및 Get API 제공~~
		* ~~클라 : 스테틱 테이블 추가 (company_for_mining, active_mining_quantity, active_mining_supply)~~
		* ~~클라 : 인스턴스 마이닝 액티브 테이블을 Update에서 폴링 요청하고, 갱신하며, 테이블들을 조합해서 Slot UI 로드명령~~
		* ~~서버 : 채굴 API 제공 : 클라로 부터 UID를 받은 후 resource_id를 이용해서 asgardium_resource_data의 Value를 참조해서 MiningPower 소모 처리하고, Supply 수량 감소시키고, Response로 UserInfo와 해당 회사의 인스턴스 마이닝 정보 전달~~
		* ~~DB : 테이블 이름변경~~
		* ~~서버 : 정적 테이블 관리방법 설계필요~~
		* ~~서버 : 초기화 부분에 어싱크 처리~~
		* ~~클라 : 초기 액티브 스크롤뷰 출력되는 타이밍 안좋음~~
		* ~~클라 : 서버연결이 안될때 예외처리해야한다.~~
		* ~~클라 : 네트워크 에러일경우 네트워크 모듈에서 처리, 아닐경우 에러 출력 모듈에서 처리~~
		* ~~클라 : 로그인 직후 서버를 끊으면 Request가 겁나게 쌓인다. 누가 SendRequest를 겁나 호출하는지 디버깅해봐야한다.~~
			* ~~클라 : 아.... 코루틴.. 무조건 데이터를 받아온다고 가정하고 짜둿구나.. 어떡하꼬?? 일시정지가 답이다.~~
			* ~~클라 : 에잇 일단 일시정지는 백로그에 넣어두고, 코루틴 돌때 callback 받고 다음 턴 돌도록 하자.~~
		* ~~클라 : https 지원되도록 socket.io 수정~~
		* ~~클라/서버 : 소켓통신 시작하자.~~
		--
		* ~~클라/서버 : 마이닝 정보 Register, Unregister API 추가~~
		* ~~서버 : Register, Unregister시 데이터 처리(컨테이너처리... 레디스가 필요한 시점이 왔다.)~~
		* ~~클라 : 소켓통신 기본적인 예외처리해야될것 확인~~
			* ~~클라 : 접속 종료 후 재접속하면 소켓찌꺼기가 남아 있다. 한번의 응답이 중복으로 온다.~~
				* ~~-> 게임오브젝트를 제거하면 되더라~~
			* ~~클라 : 지속적으로 로그가 출력되고 있는거 확인하기, 왜 로그가 출력되고 있고, 필요한 로그인가?(핑퐁로그인데 없어도 될듯)~~
	    * ~~서버/클라 : Ping Timeout으로 서버에서 연결을 끊어버린다.~~
	    	* ~~ 주기적으로 데이터를 보내주는 로직 구성해서 확인해보자. -> 그래도 disconnect된다.~~
	    	* ~~ 어떻게 해야되나???.. 아... 유니티 에디터가 액티브되어 있지 않으면 핑/퐁 응답을 하지 않는다..~~
		* ~~클라 : 소켓이 끊켰을때 재접속 처리 해줘야한다.~~
            * ~~웹소켓과 웹서버를 한통속으로 묶어주고싶다.~~
            * ~~웹소켓에서 끊킴이 발생했을때, 웹서버에서 끊킴이 발생했을때, 이것만 호출하면 재시도카운팅해주고, 콜백으로 각자 재시도를 한다.~~
            * ~~재시도 카운터는 ... ~~
            * ~~유효성검증을 위해 jwt 헤더 포함~~
            * ~~구독/구독해제 WebServer 코드 제거 소켓으로 이동~~
            * ~~소켓종료 후 Disconnect 메시지가 오기까지 딜레이가 있다.~~
            * ~~소켓 찌꺼기 문제~~

		* ~~클라/서버 : 구독, 구독해제 처리를 소켓으로 해야한다.~~
		* ~~서버 : 마이닝 정보 Registe된 클라에게 인스턴스 마이닝 정보 인터벌 주기로 소켓전송~~
			* ~~UserId와 소켓정보를 어떻게 연결시켜줘야하나??~~
			* ~~매핑테이블을 만들어야하나??~~
			* ~~소켓 컨테이너의 키는 소켓ID가 되어야하는게 맞다.~~
			* ~~그러면 소켓연결이 되었을때 유저는 웹서버로 소켓을 등록하는 요청을 보내고 소켓해제가 되었을때 등록해제하는 요청을 보내야겠다.~~
			* ~~구독 요청도 마찬가지지만 이것은 서버 스케일링에 장애물이 된다.~~
			* ~~소켓연결은 한 서버와 이루어 지지만 웹서버요청은 달라질 수 있기때문이다.~~
			* ~~그러면 소켓에 관련된 요청은 모두 소켓을통해 해야겠는데??~~
		* ~~버그~~
			* ~~클라 재접속 후 재구독 요청~~
			* ~~클라 마이닝 정보 코루틴 정지시키고 있는 버그~~
		* ~~폴리싱~~
			* ~~서버 : 인스턴스 테이블 내려주는 주기 config테이블로 빼기~~
			* ~~클라 : 소켓 클래스 정리 설계 및 개발~~
				* ~~Response 데이터 가공 (커스텀 및 폴링에서 사용)~~
			* ~~클라 : Lobby Business Login 정리 (코드가 있어야할 곳에 있도록)~~
		--
		* ~~클라 : 소켓통신으로 전달받은 마이닝 정보로 UI 처리하기~~
			* ~~소켓 이벤트(시스템, 커스텀, 폴링) 연결~~
			* ~~데이터 가공~~
				* ~~데이터 버전관리(없는 데이터도 0으로 처리하되 컨테이너는 유지, 탭이나 메뉴가 바뀌면 초기화), 데이터 정렬~~
			* ~~데이터 초기화 및 소멸화~~
				* ~~탭 이동을 기준으로 판단 (메인메뉴 이동, 마이닝 탭 이동)~~
			* ~~UI 표현~~
				* ~~가공된 데이터 대로 UI 표현~~
				* ~~세부 리스트 팝업 추가~~
            * ~~폴리싱~~
                * ~~데이터 가공 시 루프 최소화~~
		--
		* ~~클라 : Slot UI에서 구매버튼 이벤트로 채굴 API 호출하고, Response를 받아 UserInfo와 인스턴스 마이닝 액티브 테이블 업데이트 해주기~~
            * ~~인벤토리 개념 추가 필요~~
            * ~~클라 : 마이닝 시간처리정보 인벤토리에서 얻어오도록 수정~~
            * ~~서버 : 마이닝 시간처리 임시코드를 인벤토리에 있는 데이터로 갱신하도록 수정~~
            * ~~서버 : 채굴 로직 개발~~
            * ~~클라 : 스크롤뷰에 데이터 갱신 안되는 문제~~
            * ~~클라 : 안내가 필요한 서버에러에 대한 처리~~
		--
        * ~~폴리싱~~
            * ~~클라 : 스크롤뷰 갱신에 대한 문제가 있음 -> 갱신 프로세스를 변경하기로 함.(갯수변화 처리안함, 클라기준 정보업데이트만)~~
            * ~~클라 : 소켓 재접속시 재구독처리 안되는 문제~~
            * ~~서버 : NPC회사 공급물량 업데이트(매일 x시) , schedule_refresh_active_mining 0 0 10 * * *~~
            * ~~테스트 : 수량 마감에 대한 테스트~~
        --
        * 필터기능
            * 지정한 Unit에 대해서만 메인 스크롤뷰에 출력 (비휘발성)
---
* 인증
	* ~~서버/클라 : JWT 적용~~
    * ThirdParty 인증 추가 : 구글, 애플, 네이버, 라인, 페이스북, 트위터, 카카오, 구글 OTP 2차인증
---
* 푸시
    * 로컬푸시
    * 리모트푸시(파이어베이스)
---
* 기타
	* ~~String 테이블을 만들어야 한다.~~
	* 클라 : 일시정지 아이디어가 필요하다.
	* 클라 : 서버데이터 Escape 문제 해결 필요
	* 업데이트에 프로그래스 UI 만들어야 한다.
	* 글로벌UI에 디버그용 정보출력하는 패널 만들자(씬, 버전, 서비스모드)
	* ~~클라 클래스들의 역할을 명확히 해야할 필요가 있다....~~
		* ~~씬 메인과 세부 비지니스 컨트롤러, UI 루트와 UI 메니져..~~
		* ~~씬 메인은 씬의 입구와 출구를 맡는다.~~
		* ~~UI Root는 씬의 Root와 글로벌Root가 있다.~~
		* ~~세부 비지니스 로직은 각각의 UI오브젝트에 컴포넌트로 삽입되어 UI를 컨트롤한다.~~
	* 구글콘솔 앱 등록
	* 애플개발자센터 앱 등록
	* ~~젠킨스 : 안드로이드 빌드~~
	* 젠킨스 : iOS 빌드
	* 파이어베이스 앱 등록
	* ~~디플로이게이트 확인~~
---
* 소켓연결을 시도해보자.
	* ~~웹소켓 모듈을 찾자.~~
	* ~~컨넥션 및 단순 데이터 PingPong~~
	* ~~서버 웹 소켓 이벤트에 대해 조사~~
	* ~~접속 후 초기화 메시지를 추가해서 유져 authntoken과 socket.id를 매칭시켜줘야겠다.,, 이건 DB에 저장되는 값이 아님, 서버 메모리에 올라감.~~
	* ~~클라 웹 소켓 플러그인에서 출력하는 로그에 대해 조사 및 Disable~~
	* ~~예외처리방식 구상해보기~~
	* 전체 유저를 대상으로 데이터 Send
	* 특정 유저를 대상으로 데이터 Send
	* 특정 그룹을 대상으로 데이터 Send
	* ~~주기적으로 데이터 Send~~
---
* 피들러 필요하다.
---
# 고민
* ~~비지니스와 UI 구조를 어떻게 가져가면 좋을까?~~
	* 공존했을때 문제를 명확히 몰라서 잘 안짜지는거다.
		1. 공존시 코드 복잡성 증가
		2. 공존시 코드 독립성 불가능
		3. 분리시 인스팩터에서 링크해서 사용하는게 편하다.
		4. 하지만 인스팩터에서 링크했을 경우 연결고리를 찾기 힘들다.
		5. 또한 인스팩터에서 링크해서 사용하면 동적로드가 안된다.
	--
	* 코드 분리는 반드시 필요하다 OK
	* 인스팩터에서 링크를 지양하도록 하는것도 OK
	* 그러면
	--
	* 링크를 안한다면 UI 매니져를 통해 오브젝트를 얻어와야한다.
	* 단점은 깊다.
	* 비지니스 > UI매니져 > UI루트 > UI패널 > 기능까지..
	* 4단계 이상을 거쳐야 1차접근이 가능하다.
	* 더 깊다면 5~6단계를 거쳐야한다.
	* 1단계로 접근할 수 있는 방법은... 
	* 비지니스 Start에서 컨트롤 할 UI 객체를 얻어두는거 혹은 
	* UI 루트에서 객체를 Get할 수 있는 로직을 만들어 두는것..
	* UI 루트를 통해 컨트롤 명령을 내리고, UI루트에서 각 UI에 명령을 전달하는것..
	--
	* 서버랑 클라의 에러코드를 동기화해야하나?? 분리하는게 좋은거 같다.
	* 에러코드가 겹치는 문제는 영역을 분리해서 처리하는게 좋은거 같다.
	--
	* 미리 객체를 얻어두는거의 단점은 로드가 걸릴 수 있다는거..
	* var pUIRoot = await Single.UI.GetRoot<SHUIRoot>();
	* var pUIPanelXXX = pUIRoot.GetPanel<SHUIPanelXXX>();
	* var pUIPanelYYY = pUIRoot.GetPanel<SHUIPanelYYY>();
	* pUIPanelXXX.Controll
	* pUIPanelYYY.Controll
--
* ~~UI를 이름으로 관리하는 것에 대해..~~
	* 파일이름을 관리해야하는게 불편하다.
	* 그러나 타입으로 가져오면 중복에 대한 문제가 생긴다.
	* 그러나 타입으로 가져오면 파일이름을 모르기 때문에 동적로드에 대한 문제가 생긴다.
--
	* 그냥 파일이름으로 가야겠다.
--
* 로컬 테스트시 서버설정이 불편한 점이 있네..
	* 로컬에 DB를 실행시키고,
	* 서버의 컨피그(URL)를 수정하고,
	* 서버를 실행시키고,
	* 클라의 컨피그(URL)를 수정하고,
	* 테스트 진행 가능
	* 작업 진행 후 Git Push하고,
	* AWS 서버접속해서 서버 스톱하고, Git Pull하고, 서버실행하고, 로그켜고,
	* 클라 Git Push하고,
	* 젠킨스 빌드 돌리고,
	* 클라 실행해서 동작확인
--