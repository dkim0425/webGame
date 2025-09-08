from fastapi import FastAPI, WebSocket
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # 테스트용
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.websocket("/ws/chat")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    print("클라이언트 연결됨")

    await websocket.send_text("안녕 난 서버")

    while True:
        try:
            data = await websocket.receive_text()
            print(f"Unity로부터 받은 메시지: {data}")
            await websocket.send_text(f"서버가 받음: {data}")
        except:
            print("클라이언트 연결 종료")
            break
